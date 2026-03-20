namespace Warehome.Web.DTO.Input;

public class CreateItemTypeRequest
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}