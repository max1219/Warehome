namespace Warehome.Application.DTO.Input;

public class DeleteStorageCommand
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}