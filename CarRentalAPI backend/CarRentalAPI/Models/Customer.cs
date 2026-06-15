namespace CarRentalAPI.Models;

public class Customer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public required string LicenseNo { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
