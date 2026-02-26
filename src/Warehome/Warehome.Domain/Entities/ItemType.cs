namespace Warehome.Domain.Entities;

public class ItemType
{
    public string Name { get; set; }
    public Category<ItemType>? Category { get; set; }
    public string? Description { get; set; }
    
}