namespace Warehome.Infrastructure.Data.Entities;

public class ItemTypeCategory
{
    public int Id { get; set; }
    public string Path { get; set; }
    public int? ParentId { get; set; }
    public ItemTypeCategory? Parent { get; set; }
}