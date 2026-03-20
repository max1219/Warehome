namespace Warehome.Web.DTO.Input;

public class DeleteStorageRequest
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}