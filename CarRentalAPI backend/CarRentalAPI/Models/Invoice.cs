namespace CarRentalAPI.Models;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public int? AgreementId { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime StartDate { get; set; }
    public int Period { get; set; }                    // number of days
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";   // Pending, Generated, Paid, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? GeneratedAt { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
    public Agreement? Agreement { get; set; }
}
