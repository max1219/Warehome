// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Warehome.Domain.Entities;

public class ItemStock
{
    public int Id { get; set; }
    public required ItemType ItemType { get; set; }
    public required Storage Storage { get; set; }
    public required int Quantity { get; set; }
}