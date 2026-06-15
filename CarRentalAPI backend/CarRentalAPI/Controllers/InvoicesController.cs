using System.Security.Claims;
using CarRentalAPI.Data;
using CarRentalAPI.DTOs;
using CarRentalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public InvoicesController(AppDbContext db) => _db = db;

    /// <summary>Get all invoices. Admin sees all; Customer sees own.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var customerIdClaim = User.FindFirstValue("CustomerId");

        var query = _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Vehicle)
            .AsQueryable();

        if (role == "Customer" && int.TryParse(customerIdClaim, out int cid))
            query = query.Where(i => i.CustomerId == cid);

        var result = await query
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => MapToResponse(i))
            .ToListAsync();

        return Ok(ApiResponse<List<InvoiceResponseDto>>.Ok(result));
    }

    ///get invoices by id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound(ApiResponse<string>.Fail("Invoice not found."));

        var role = User.FindFirstValue(ClaimTypes.Role);
        var customerIdClaim = User.FindFirstValue("CustomerId");

        if (role != "Admin" && customerIdClaim != invoice.CustomerId.ToString())
            return Forbid();

        return Ok(ApiResponse<InvoiceResponseDto>.Ok(MapToResponse(invoice)));
    }

    /// create new invoce
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InvoiceCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        var customer = await _db.Customers.FindAsync(dto.CustomerId);
        if (customer == null || !customer.IsActive)
            return BadRequest(ApiResponse<string>.Fail("Customer not found or inactive."));

        var vehicle = await _db.Vehicles.FindAsync(dto.VehicleId);
        if (vehicle == null) return BadRequest(ApiResponse<string>.Fail("Vehicle not found."));

        var invoiceNo = GenerateInvoiceNumber();

        var invoice = new Invoice
        {
            InvoiceNo = invoiceNo,
            CustomerId = dto.CustomerId,
            VehicleId = dto.VehicleId,
            AgreementId = dto.AgreementId,
            PlateNo = dto.PlateNo.ToUpper(),
            Rate = dto.Rate,
            StartDate = dto.StartDate,
            Period = dto.Period,
            TotalAmount = dto.Rate * dto.Period,
            Status = "Pending"
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        await _db.Entry(invoice).Reference(i => i.Customer).LoadAsync();
        await _db.Entry(invoice).Reference(i => i.Vehicle).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id },
            ApiResponse<InvoiceResponseDto>.Ok(MapToResponse(invoice), "Invoice created."));
    }

    /// <summary>Update an invoice. [Admin] - Only allowed while Pending.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] InvoiceUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        var invoice = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound(ApiResponse<string>.Fail("Invoice not found."));
        if (invoice.Status == "Generated" || invoice.Status == "Paid")
            return BadRequest(ApiResponse<string>.Fail("Cannot edit a generated or paid invoice."));

        var validStatuses = new[] { "Pending", "Generated", "Paid", "Cancelled" };
        if (!validStatuses.Contains(dto.Status))
            return BadRequest(ApiResponse<string>.Fail("Invalid status."));

        invoice.CustomerId = dto.CustomerId;
        invoice.VehicleId = dto.VehicleId;
        invoice.AgreementId = dto.AgreementId;
        invoice.PlateNo = dto.PlateNo.ToUpper();
        invoice.Rate = dto.Rate;
        invoice.StartDate = dto.StartDate;
        invoice.Period = dto.Period;
        invoice.TotalAmount = dto.Rate * dto.Period;
        invoice.Status = dto.Status;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<InvoiceResponseDto>.Ok(MapToResponse(invoice), "Invoice updated."));
    }

    /// <summary>Generate (finalize) an invoice. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/generate")]
    public async Task<IActionResult> Generate(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound(ApiResponse<string>.Fail("Invoice not found."));
        if (invoice.Status != "Pending")
            return BadRequest(ApiResponse<string>.Fail("Only pending invoices can be generated."));

        invoice.Status = "Generated";
        invoice.GeneratedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<InvoiceResponseDto>.Ok(MapToResponse(invoice), "Invoice generated."));
    }

    /// <summary>Mark invoice as Paid. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> MarkPaid(int id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound(ApiResponse<string>.Fail("Invoice not found."));
        if (invoice.Status != "Generated")
            return BadRequest(ApiResponse<string>.Fail("Invoice must be generated before marking paid."));

        invoice.Status = "Paid";
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<InvoiceResponseDto>.Ok(MapToResponse(invoice), "Invoice marked as paid."));
    }

    /// <summary>Delete an invoice. [Admin] - Only Pending invoices.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice == null) return NotFound(ApiResponse<string>.Fail("Invoice not found."));

        if (invoice.Status != "Pending")
            return BadRequest(ApiResponse<string>.Fail("Only pending invoices can be deleted."));

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.Ok("Deleted", "Invoice deleted."));
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }

    private static InvoiceResponseDto MapToResponse(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNo = i.InvoiceNo,
        CustomerId = i.CustomerId,
        CustomerName = i.Customer?.FullName ?? string.Empty,
        VehicleId = i.VehicleId,
        VehicleName = i.Vehicle != null ? $"{i.Vehicle.Make} {i.Vehicle.Model}" : string.Empty,
        AgreementId = i.AgreementId,
        PlateNo = i.PlateNo,
        Rate = i.Rate,
        StartDate = i.StartDate,
        Period = i.Period,
        TotalAmount = i.TotalAmount,
        Status = i.Status,
        CreatedAt = i.CreatedAt,
        GeneratedAt = i.GeneratedAt
    };
}
