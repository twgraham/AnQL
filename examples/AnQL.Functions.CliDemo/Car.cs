using AnQL.Core.Attributes;

namespace AnQL.Functions.CliDemo;

public record Car
{
    public string Make { get; set; }
    
    public string Model { get; set; }
    
    [AnQLProperty("Doors")]
    public int NumberOfDoors { get; set; }
    
    public decimal Price { get; set; }
    
    public DateTime InitialRelease { get; set; }
}