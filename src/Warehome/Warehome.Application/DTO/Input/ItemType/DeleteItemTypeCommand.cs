namespace Warehome.Application.DTO.Input;

public class DeleteItemTypeCommand
{
    public string Name { get; init; }
    public string? CategoryPath { get; init; }
}