using CarRentalAPI.Data;
using CarRentalAPI.DTOs;
using CarRentalAPI.Helpers;
using CarRentalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthController(AppDbContext db, JwtHelper jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    // Regestring a new cusomter
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(ApiResponse<string>.Fail("Email already registered."));

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

        // Auto-create a customer profile
        var customer = new Customer
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Address = string.Empty,
            LicenseNo = dto.LicenseNo,  
            NationalId = string.Empty
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(user, customer.Id);
        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            Role = user.Role,
            FullName = user.FullName,
            UserId = user.Id,
            CustomerId = customer.Id,
             LicenseNo = dto.LicenseNo
        }, "Registration successful."));
    }

    /// <summary>Login for both Admin and Customer.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        var user = await _db.Users
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(ApiResponse<string>.Fail("Invalid credentials."));

        if (!user.IsActive)
            return Unauthorized(ApiResponse<string>.Fail("Account is disabled."));

        var token = _jwt.GenerateToken(user, user.Customer?.Id);
        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            Role = user.Role,
            FullName = user.FullName,
            UserId = user.Id,
            CustomerId = user.Customer?.Id
        }));
    }
}
