namespace Warehome.Web.DTO.Input;

public class GetItemStocksByStorageRequest
{
    public string StorageName { get; set; }
    public string? StorageCategoryPath { get; set; }
}