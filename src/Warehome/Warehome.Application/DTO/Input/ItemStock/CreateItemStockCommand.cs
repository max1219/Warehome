namespace Warehome.Application.DTO.Input;

public class CreateItemStockCommand
{
    public required string ItemTypeName { get; init; }
    public string? ItemTypeCategoryPath { get; init; }
    public required string StorageName { get; init; }
    public string? StorageCategoryPath { get; init; }
    public required int Quantity { get; init; }
}