namespace Warehome.Web.DTO.Input;

public class GetItemStocksByTypeRequest
{
    public required string ItemTypeName { get; init; }
    public string? ItemTypeCategoryPath { get; init; }
}