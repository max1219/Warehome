namespace Warehome.Infrastructure.Data.Entities;

public class ItemType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? CategoryId { get; set; }
    public ItemTypeCategory? Category { get; set; }
}