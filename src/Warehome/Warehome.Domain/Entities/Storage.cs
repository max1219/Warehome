// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Warehome.Domain.Entities;

public class Storage
{
    public required string Name { get; set; }
    public Category<Storage>? Category { get; set; }
}