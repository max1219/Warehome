namespace Warehome.Web.DTO.Input;

public class CreateItemStockRequest
{
    public required string ItemTypeName { get; init; }
    public string? ItemTypeCategoryPath { get; init; }
    public required string StorageName { get; init; }
    public string? StorageCategoryPath { get; init; }
    public required int Quantity { get; init; }
}