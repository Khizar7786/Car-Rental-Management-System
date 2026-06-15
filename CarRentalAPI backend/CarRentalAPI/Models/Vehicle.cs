namespace CarRentalAPI.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;         // e.g. Suzuki
    public string Model { get; set; } = string.Empty;        // e.g. Alto
    public int Year { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;     // Hatchback, Sedan, SUV, Bundles
    public decimal PerDayRate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
