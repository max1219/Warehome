namespace Warehome.Application.DTO.Input;

public class DeleteStorageCommand
{
    public string Name { get; init; }
    public string? CategoryPath { get; init; }
}