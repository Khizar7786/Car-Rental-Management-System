using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.DTOs;

// Auth 
public class RegisterDto
{
    [Required] [StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] [MinLength(8)] public string Password { get; set; } = string.Empty;
    [Required] [Phone] public string Phone { get; set; } = string.Empty;
    [Required] [StringLength(100)] public string LicenseNo { get; set; }
}

public class LoginDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? CustomerId { get; set; }
    public string LicenseNo { get; set; }
}

// Vehicle

public class VehicleCreateDto
{
    [Required] [StringLength(50)] public string Make { get; set; } = string.Empty;
    [Required] [StringLength(50)] public string Model { get; set; } = string.Empty;
    [Range(2000, 2030)] public int Year { get; set; }
    [Required] [StringLength(20)] public string PlateNo { get; set; } = string.Empty;
    [Required] public string Category { get; set; } = string.Empty;  // Hatchback, Sedan, SUV, Bundles
    [Range(1, 1000000)] public decimal PerDayRate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    [StringLength(500)] public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}

public class VehicleUpdateDto : VehicleCreateDto { }

public class VehicleResponseDto
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal PerDayRate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Customer 

public class CustomerCreateDto
{
    [Required] [StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] [MinLength(8)] public string Password { get; set; } = string.Empty;
    [Required] [Phone] public string Phone { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string Address { get; set; } = string.Empty;
    [Required] [StringLength(30)] public string LicenseNo { get; set; } = string.Empty;
    [Required] [StringLength(20)] public string NationalId { get; set; } = string.Empty;
}

public class CustomerUpdateDto
{
    [Required] [StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required] [Phone] public string Phone { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string Address { get; set; } = string.Empty;
    [Required] [StringLength(30)] public string LicenseNo { get; set; } = string.Empty;
    [Required] [StringLength(20)] public string NationalId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class CustomerResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string LicenseNo { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Agreement 

public class AgreementCreateDto
{
    [Required] public int CustomerId { get; set; }
    [Required] public int VehicleId { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [StringLength(500)] public string Notes { get; set; } = string.Empty;
}

public class AgreementUpdateDto
{
    [Required] public int CustomerId { get; set; }
    [Required] public int VehicleId { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Required] public string Status { get; set; } = "Active";
    [StringLength(500)] public string Notes { get; set; } = string.Empty;
}

public class AgreementResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public string PlateNo { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal RatePerDay { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Invoice 

public class InvoiceCreateDto
{
    [Required] public int CustomerId { get; set; }
    [Required] public int VehicleId { get; set; }
    public int? AgreementId { get; set; }
    [Required] [StringLength(20)] public string PlateNo { get; set; } = string.Empty;
    [Range(1, 1000000)] public decimal Rate { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Range(1, 365)] public int Period { get; set; }
}

public class InvoiceUpdateDto
{
    [Required] public int CustomerId { get; set; }
    [Required] public int VehicleId { get; set; }
    public int? AgreementId { get; set; }
    [Required] [StringLength(20)] public string PlateNo { get; set; } = string.Empty;
    [Range(1, 1000000)] public decimal Rate { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Range(1, 365)] public int Period { get; set; }
    [Required] public string Status { get; set; } = "Pending";
}

public class InvoiceResponseDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public int? AgreementId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime StartDate { get; set; }
    public int Period { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? GeneratedAt { get; set; }
}

// Generic 

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
