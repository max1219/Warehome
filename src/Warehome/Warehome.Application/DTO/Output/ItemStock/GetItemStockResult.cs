namespace Warehome.Application.DTO.Output;

public class GetItemStockResult
{
    public required int Id { get; init; }
    public required string ItemTypeName { get; init; }
    public string? ItemTypeCategoryPath { get; init; }
    public required string StorageName { get; init; }
    public string? StorageCategoryPath { get; init; }
    public required int Quantity { get; init; }

    public static GetItemStockResult FromItemStock(Domain.Entities.ItemStock itemStock)
    {
        return new GetItemStockResult
        {
            Id = itemStock.Id,
            Quantity = itemStock.Quantity,
            StorageName = itemStock.Storage.Name,
            StorageCategoryPath = itemStock.Storage.Category?.Name,
            ItemTypeName = itemStock.ItemType.Name,
            ItemTypeCategoryPath = itemStock.ItemType.Category?.Name,
        };
    }
}