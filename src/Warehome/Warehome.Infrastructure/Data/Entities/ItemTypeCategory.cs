// ReSharper disable PropertyCanBeMadeInitOnly.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Warehome.Infrastructure.Data.Entities;

public class ItemTypeCategory
{
    public int Id { get; set; }
    public string Path { get; set; }
    public int? ParentId { get; set; }
    public ItemTypeCategory? Parent { get; set; }
}