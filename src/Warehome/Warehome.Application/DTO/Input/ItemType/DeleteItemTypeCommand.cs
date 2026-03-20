namespace Warehome.Application.DTO.Input;

public class DeleteItemTypeCommand
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}