namespace Warehome.Web.DTO.Input;

public class CreateItemTypeCategoryRequest
{
    public required string Name { get; init; }
    public string? ParentPath { get; init; }
}