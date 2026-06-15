namespace CarRentalAPI.Models;

public class Agreement
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal RatePerDay { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Active";   // Active, Completed, Cancelled
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
