namespace Warehome.Application.DTO.Input;

public class CreateItemTypeCommand
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}