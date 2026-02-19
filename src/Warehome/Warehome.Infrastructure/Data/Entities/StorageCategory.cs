namespace Warehome.Infrastructure.Data.Entities;

public class StorageCategory
{
    public int Id { get; set; }
    public string Path { get; set; }
    public int? ParentId { get; set; }
    public StorageCategory? Parent { get; set; }
}