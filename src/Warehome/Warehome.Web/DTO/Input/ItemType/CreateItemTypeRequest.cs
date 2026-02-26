namespace Warehome.Web.DTO.Input;

public class CreateItemTypeRequest
{
    public string Name { get; init; }
    public string? CategoryPath { get; init; }
}