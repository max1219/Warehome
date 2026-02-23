namespace Warehome.Application.DTO.Input;

public class CreateStorageCommand
{
    public string Name { get; init; }
    public string? CategoryPath { get; init; }
}