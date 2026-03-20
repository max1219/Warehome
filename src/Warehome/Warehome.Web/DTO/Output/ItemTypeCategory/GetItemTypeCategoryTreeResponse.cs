namespace Warehome.Web.DTO.Output;

public class GetItemTypeCategoryTreeResponse
{
    public required string Name { get; init; }
    public required IReadOnlyList<GetItemTypeCategoryTreeResponse> Children { get; init; }
    public required int ChildCount { get; init; }
    public required IReadOnlyList<string> ItemNames { get; init; }
    public required int ItemTypeCount { get; init; }
}