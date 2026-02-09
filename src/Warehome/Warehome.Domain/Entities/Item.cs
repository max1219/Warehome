namespace Warehome.Domain.Entities;

public class Item
{
    public string Name { get; set; }
    public ItemCategory? Category { get; set; }
    public string? Description { get; set; }
    
}