namespace Warehome.Web.DTO.Input;

public class CreateItemTypeCategoryRequest
{
    public string Name { get; init; }
    public string? ParentPath { get; init; }
}