namespace Warehome.Application.DTO.Output;

public class GetStorageResult
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}