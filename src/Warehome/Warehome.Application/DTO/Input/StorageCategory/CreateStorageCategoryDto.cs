namespace Warehome.Application.DTO.Input;

public class CreateStorageCategoryDto
{
    public string Name { get; init; }
    public string? ParentPath { get; init; }
}