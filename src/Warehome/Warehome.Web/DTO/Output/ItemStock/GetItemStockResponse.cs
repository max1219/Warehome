namespace Warehome.Web.DTO.Output;

public class GetItemStockResponse
{
    public int Id { get; set; }
    public string ItemTypeName { get; set; }
    public string? ItemTypeCategoryPath { get; set; }
    public string StorageName { get; set; }
    public string? StorageCategoryPath { get; set; }
    public int Quantity { get; set; }
}