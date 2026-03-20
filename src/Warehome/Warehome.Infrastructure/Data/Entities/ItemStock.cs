// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Warehome.Infrastructure.Data.Entities;

public class ItemStock
{
    public int Id { get; set; }
    public ItemType ItemType { get; set; }
    public int ItemTypeId { get; set; }
    public Storage Storage { get; set; }
    public int StorageId { get; set; }
    public int Quantity { get; set; }
}