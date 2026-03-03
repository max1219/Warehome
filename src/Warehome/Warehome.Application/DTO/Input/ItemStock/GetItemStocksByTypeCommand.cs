namespace Warehome.Application.DTO.Input;

public class GetItemStocksByTypeCommand
{
    public string ItemTypeName { get; set; }
    public string? ItemTypeCategoryPath { get; set; }
}