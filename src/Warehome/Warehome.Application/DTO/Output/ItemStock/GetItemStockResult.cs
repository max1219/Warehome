namespace Warehome.Application.DTO.Output;

public class GetItemStockResult
{
    public int Id { get; set; }
    public string ItemTypeName { get; set; }
    public string? ItemTypeCategoryPath { get; set; }
    public string StorageName { get; set; }
    public string? StorageCategoryPath { get; set; }
    public int Quantity { get; set; }

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