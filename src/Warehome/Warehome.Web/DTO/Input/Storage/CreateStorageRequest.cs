namespace Warehome.Web.DTO.Input;

public class CreateStorageRequest
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}