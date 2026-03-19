namespace Warehome.Web.DTO.Input;

public class CreateItemStockRequest
{
    public string ItemTypeName { get; set; }
    public string? ItemTypeCategoryPath { get; set; }
    public string StorageName { get; set; }
    public string? StorageCategoryPath { get; set; }
    public int Quantity { get; set; }
}