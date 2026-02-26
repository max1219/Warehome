namespace Warehome.Application.DTO.Input;

public class CreateItemTypeCategoryCommand
{
    public string Name { get; init; }
    public string? ParentPath { get; init; }
}