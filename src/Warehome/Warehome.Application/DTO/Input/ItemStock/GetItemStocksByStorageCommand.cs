namespace Warehome.Application.DTO.Input;

public class GetItemStocksByStorageCommand
{
    public required string StorageName { get; init; }
    public string? StorageCategoryPath { get; init; }
}