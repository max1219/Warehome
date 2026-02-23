namespace Warehome.Web.DTO.Input;

public class DeleteStorageRequest
{
    public string Name { get; init; }
    public string? CategoryPath { get; init; }
}