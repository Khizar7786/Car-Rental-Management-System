using CarRentalAPI.Data;
using CarRentalAPI.DTOs;
using CarRentalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _db;

    public VehiclesController(AppDbContext db) => _db = db;

 

    /// get all vehichles optionally by category
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] bool? available)
    {
        var query = _db.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(v => v.Category == category);

        if (available.HasValue)
            query = query.Where(v => v.IsAvailable == available.Value);

        var vehicles = await query
            .OrderBy(v => v.Make)
            .Select(v => MapToResponse(v))
            .ToListAsync();

        return Ok(ApiResponse<List<VehicleResponseDto>>.Ok(vehicles));
    }

    //get a single vehichle by id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var v = await _db.Vehicles.FindAsync(id);
        if (v == null) return NotFound(ApiResponse<string>.Fail("Vehicle not found."));
        return Ok(ApiResponse<VehicleResponseDto>.Ok(MapToResponse(v)));
    }

 

    // cereating a new vehichle
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        if (await _db.Vehicles.AnyAsync(v => v.PlateNo == dto.PlateNo))
            return Conflict(ApiResponse<string>.Fail("Plate number already exists."));

        var validCategories = new[] { "Hatchback", "Sedan", "SUV", "Bundles" };
        if (!validCategories.Contains(dto.Category))
            return BadRequest(ApiResponse<string>.Fail("Invalid category. Use: Hatchback, Sedan, SUV, Bundles."));

        var vehicle = new Vehicle
        {
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            PlateNo = dto.PlateNo.ToUpper(),
            Category = dto.Category,
            PerDayRate = dto.PerDayRate,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description,
            IsAvailable = dto.IsAvailable
        };

        _db.Vehicles.Add(vehicle);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id },
            ApiResponse<VehicleResponseDto>.Ok(MapToResponse(vehicle), "Vehicle created."));
    }

    // for updating a vehichle
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] VehicleUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input."));

        var vehicle = await _db.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound(ApiResponse<string>.Fail("Vehicle not found."));

        if (await _db.Vehicles.AnyAsync(v => v.PlateNo == dto.PlateNo && v.Id != id))
            return Conflict(ApiResponse<string>.Fail("Plate number already in use."));

        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.PlateNo = dto.PlateNo.ToUpper();
        vehicle.Category = dto.Category;
        vehicle.PerDayRate = dto.PerDayRate;
        vehicle.ImageUrl = dto.ImageUrl;
        vehicle.Description = dto.Description;
        vehicle.IsAvailable = dto.IsAvailable;

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<VehicleResponseDto>.Ok(MapToResponse(vehicle), "Vehicle updated."));
    }

    /// deleting a vehichle
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _db.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound(ApiResponse<string>.Fail("Vehicle not found."));

        bool hasRelated = await _db.Agreements.AnyAsync(a => a.VehicleId == id) ||
                          await _db.Invoices.AnyAsync(i => i.VehicleId == id);

        if (hasRelated)
            return BadRequest(ApiResponse<string>.Fail("Cannot delete: vehicle has linked agreements or invoices."));

        _db.Vehicles.Remove(vehicle);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.Ok("Deleted", "Vehicle deleted."));
    }

    private static VehicleResponseDto MapToResponse(Vehicle v) => new()
    {
        Id = v.Id,
        Make = v.Make,
        Model = v.Model,
        Year = v.Year,
        PlateNo = v.PlateNo,
        Category = v.Category,
        PerDayRate = v.PerDayRate,
        ImageUrl = v.ImageUrl,
        Description = v.Description,
        IsAvailable = v.IsAvailable,
        CreatedAt = v.CreatedAt
    };
}
