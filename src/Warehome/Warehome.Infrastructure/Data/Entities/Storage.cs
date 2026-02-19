namespace Warehome.Infrastructure.Data.Entities;

public class Storage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? CategoryId { get; set; }
    public StorageCategory? Category { get; set; }
}