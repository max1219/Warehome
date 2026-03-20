namespace Warehome.Application.DTO.Output;

public class GetItemTypeResult
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}