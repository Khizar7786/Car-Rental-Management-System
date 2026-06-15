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
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db) => _db = db;

    // ── ADMIN ────────────────────────────────────────────────────────────────

    /// <summary>Get all customers. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _db.Customers
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => MapToResponse(c))
            .ToListAsync();

        return Ok(ApiResponse<List<CustomerResponseDto>>.Ok(customers));
    }

    /// <summary>Get customer by ID. [Admin or own profile]</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var customerIdClaim = User.FindFirstValue("CustomerId");

        // Non-admins can only view their own profile
        if (role != "Admin" && customerIdClaim != id.ToString())
            return Forbid();

        var c = await _db.Customers.FindAsync(id);
        if (c == null) return NotFound(ApiResponse<string>.Fail("Customer not found."));
        return Ok(ApiResponse<CustomerResponseDto>.Ok(MapToResponse(c)));
    }

    /// <summary>Create customer (Admin only - for walk-in customers).</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(ApiResponse<string>.Fail("Email already registered."));

        if (await _db.Customers.AnyAsync(c => c.LicenseNo == dto.LicenseNo))
            return Conflict(ApiResponse<string>.Fail("License number already registered."));

        // Create user account
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Phone = dto.Phone,
            Role = "Customer"
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Create customer profile
        var customer = new Customer
        {
            UserId = user.Id,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            LicenseNo = dto.LicenseNo,
            NationalId = dto.NationalId
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id },
            ApiResponse<CustomerResponseDto>.Ok(MapToResponse(customer), "Customer created."));
    }

    /// <summary>Update customer. [Admin or own profile]</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        var role = User.FindFirstValue(ClaimTypes.Role);
        var customerIdClaim = User.FindFirstValue("CustomerId");

        if (role != "Admin" && customerIdClaim != id.ToString())
            return Forbid();

        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return NotFound(ApiResponse<string>.Fail("Customer not found."));

        if (await _db.Customers.AnyAsync(c => c.LicenseNo == dto.LicenseNo && c.Id != id))
            return Conflict(ApiResponse<string>.Fail("License number already in use."));

        customer.FullName = dto.FullName;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        customer.LicenseNo = dto.LicenseNo;
        customer.NationalId = dto.NationalId;

        // Only admin can toggle active status
        if (role == "Admin")
            customer.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<CustomerResponseDto>.Ok(MapToResponse(customer), "Customer updated."));
    }

    /// <summary>Soft-delete (deactivate) a customer. [Admin]</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null) return NotFound(ApiResponse<string>.Fail("Customer not found."));

        // Soft delete
        customer.IsActive = false;
        customer.User.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deactivated", "Customer deactivated."));
    }

    private static CustomerResponseDto MapToResponse(Customer c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        FullName = c.FullName,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        LicenseNo = c.LicenseNo,
        NationalId = c.NationalId,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    };
}
