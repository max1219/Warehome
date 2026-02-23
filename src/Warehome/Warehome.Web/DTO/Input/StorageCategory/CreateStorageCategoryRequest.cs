namespace Warehome.Web.DTO.Input;

public class CreateStorageCategoryRequest
{
    public string Name { get; init; }
    public string? ParentPath { get; init; }
}