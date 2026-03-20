namespace Warehome.Web.DTO.Input;

public class DeleteItemTypeRequest
{
    public required string Name { get; init; }
    public string? CategoryPath { get; init; }
}