namespace Warehome.Web.DTO.Input;

public class GetItemStocksByStorageRequest
{
    public required string StorageName { get; init; }
    public string? StorageCategoryPath { get; init; }
}