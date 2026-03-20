namespace Warehome.Application.DTO.Input;

public class CreateStorageCommand
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}