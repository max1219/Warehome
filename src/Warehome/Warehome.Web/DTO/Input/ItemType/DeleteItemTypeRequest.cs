namespace Warehome.Web.DTO.Input;

public class DeleteItemTypeRequest
{
    public string Name { get; init; }
    public string? CategoryPath { get; init; }
}