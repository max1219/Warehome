namespace Warehome.Application.DTO.Input;

public class GetItemStocksByTypeCommand
{
    public required string ItemTypeName { get; init; }
    public string? ItemTypeCategoryPath { get; init; }
}