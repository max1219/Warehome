namespace Warehome.Web.DTO.Input;

public class CreateStorageCategoryRequest
{
    public required string Name { get; init; }
    public string? ParentPath { get; init; }
}