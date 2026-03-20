// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Warehome.Domain.Entities;

public class ItemType
{
    public required string Name { get; set; }
    public Category<ItemType>? Category { get; set; }
    
}