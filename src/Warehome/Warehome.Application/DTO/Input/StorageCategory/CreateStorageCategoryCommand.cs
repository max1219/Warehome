namespace Warehome.Application.DTO.Input;

public class CreateStorageCategoryCommand
{
    public string Name { get; init; }
    public string? ParentPath { get; init; }
}