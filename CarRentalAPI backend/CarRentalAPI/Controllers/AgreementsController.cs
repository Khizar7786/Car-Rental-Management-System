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
public class AgreementsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AgreementsController(AppDbContext db) => _db = db;

    /// <summary>Get all agreements. Admin sees all; Customer sees own.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var customerIdClaim = User.FindFirstValue("CustomerId");

        var query = _db.Agreements
            .Include(a => a.Customer)
            .Include(a => a.Vehicle)
            .AsQueryable();

        if (role == "Customer" && int.TryParse(customerIdClaim, out int cid))
            query = query.Where(a => a.CustomerId == cid);

        var result = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToResponse(a))
            .ToListAsync();

        return Ok(ApiResponse<List<AgreementResponseDto>>.Ok(result));
    }

    /// <summary>Get agreement by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var a = await _db.Agreements
            .Include(x => x.Customer)
            .Include(x => x.Vehicle)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a == null) return NotFound(ApiResponse<string>.Fail("Agreement not found."));

        var role = User.FindFirstValue(ClaimTypes.Role);
        var customerIdClaim = User.FindFirstValue("CustomerId");

        if (role != "Admin" && customerIdClaim != a.CustomerId.ToString())
            return Forbid();

        return Ok(ApiResponse<AgreementResponseDto>.Ok(MapToResponse(a)));
    }

    /// <summary>Create a new rental agreement. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AgreementCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        if (dto.EndDate <= dto.StartDate)
            return BadRequest(ApiResponse<string>.Fail("End date must be after start date."));

        var customer = await _db.Customers.FindAsync(dto.CustomerId);
        if (customer == null || !customer.IsActive)
            return BadRequest(ApiResponse<string>.Fail("Customer not found or inactive."));

        var vehicle = await _db.Vehicles.FindAsync(dto.VehicleId);
        if (vehicle == null) return BadRequest(ApiResponse<string>.Fail("Vehicle not found."));
        if (!vehicle.IsAvailable)
            return BadRequest(ApiResponse<string>.Fail("Vehicle is not available."));

        // Check vehicle isn't already booked for overlapping dates
        bool overlap = await _db.Agreements.AnyAsync(a =>
            a.VehicleId == dto.VehicleId &&
            a.Status == "Active" &&
            a.StartDate < dto.EndDate &&
            a.EndDate > dto.StartDate);

        if (overlap)
            return Conflict(ApiResponse<string>.Fail("Vehicle already booked for selected dates."));

        int days = (dto.EndDate - dto.StartDate).Days;
        var agreement = new Agreement
        {
            CustomerId = dto.CustomerId,
            VehicleId = dto.VehicleId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RatePerDay = vehicle.PerDayRate,
            TotalAmount = vehicle.PerDayRate * days,
            Status = "Active",
            Notes = dto.Notes
        };

        // Mark vehicle unavailable
        vehicle.IsAvailable = false;

        _db.Agreements.Add(agreement);
        await _db.SaveChangesAsync();

        await _db.Entry(agreement).Reference(a => a.Customer).LoadAsync();
        await _db.Entry(agreement).Reference(a => a.Vehicle).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = agreement.Id },
            ApiResponse<AgreementResponseDto>.Ok(MapToResponse(agreement), "Agreement created."));
    }

    /// <summary>Update an agreement. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AgreementUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        if (dto.EndDate <= dto.StartDate)
            return BadRequest(ApiResponse<string>.Fail("End date must be after start date."));

        var agreement = await _db.Agreements
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (agreement == null) return NotFound(ApiResponse<string>.Fail("Agreement not found."));

        var validStatuses = new[] { "Active", "Completed", "Cancelled" };
        if (!validStatuses.Contains(dto.Status))
            return BadRequest(ApiResponse<string>.Fail("Invalid status."));

        // If completing/cancelling, free up the vehicle
        if ((dto.Status == "Completed" || dto.Status == "Cancelled") && agreement.Status == "Active")
            agreement.Vehicle.IsAvailable = true;

        int days = (dto.EndDate - dto.StartDate).Days;
        agreement.CustomerId = dto.CustomerId;
        agreement.VehicleId = dto.VehicleId;
        agreement.StartDate = dto.StartDate;
        agreement.EndDate = dto.EndDate;
        agreement.RatePerDay = agreement.Vehicle.PerDayRate;
        agreement.TotalAmount = agreement.Vehicle.PerDayRate * days;
        agreement.Status = dto.Status;
        agreement.Notes = dto.Notes;

        await _db.SaveChangesAsync();

        await _db.Entry(agreement).Reference(a => a.Customer).LoadAsync();

        return Ok(ApiResponse<AgreementResponseDto>.Ok(MapToResponse(agreement), "Agreement updated."));
    }

    /// <summary>Delete an agreement. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var agreement = await _db.Agreements
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agreement == null) return NotFound(ApiResponse<string>.Fail("Agreement not found."));

        if (agreement.Status == "Active")
            agreement.Vehicle.IsAvailable = true;

        _db.Agreements.Remove(agreement);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.Ok("Deleted", "Agreement deleted."));
    }

    private static AgreementResponseDto MapToResponse(Agreement a) => new()
    {
        Id = a.Id,
        CustomerId = a.CustomerId,
        CustomerName = a.Customer?.FullName ?? string.Empty,
        VehicleId = a.VehicleId,
        VehicleName = a.Vehicle != null ? $"{a.Vehicle.Make} {a.Vehicle.Model} ({a.Vehicle.Year})" : string.Empty,
        PlateNo = a.Vehicle?.PlateNo ?? string.Empty,
        StartDate = a.StartDate,
        EndDate = a.EndDate,
        RatePerDay = a.RatePerDay,
        TotalAmount = a.TotalAmount,
        Status = a.Status,
        Notes = a.Notes,
        CreatedAt = a.CreatedAt
    };
}
