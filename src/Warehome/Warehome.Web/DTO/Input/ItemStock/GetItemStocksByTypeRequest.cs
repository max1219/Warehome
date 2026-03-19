namespace Warehome.Web.DTO.Input;

public class GetItemStocksByTypeRequest
{
    public string ItemTypeName { get; set; }
    public string? ItemTypeCategoryPath { get; set; }
}