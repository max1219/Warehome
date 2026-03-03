namespace Warehome.Application.DTO.Input;

public class GetItemStocksByStorageCommand
{
    public string StorageName { get; set; }
    public string? StorageCategoryPath { get; set; }
}