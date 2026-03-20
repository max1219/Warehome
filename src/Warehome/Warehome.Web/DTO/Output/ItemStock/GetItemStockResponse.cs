namespace Warehome.Web.DTO.Output;

public class GetItemStockResponse
{
    public required int Id { get; init; }
    public required string ItemTypeName { get; init; }
    public string? ItemTypeCategoryPath { get; init; }
    public required string StorageName { get; init; }
    public string? StorageCategoryPath { get; init; }
    public required int Quantity { get; init; }
}