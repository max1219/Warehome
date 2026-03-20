namespace Warehome.Application.DTO.Output;

public class GetItemTypeCategoryTreeResult
{
    public required string Name { get; init; }

    public IReadOnlyList<GetItemTypeCategoryTreeResult> Children { get; set; } =
        Array.Empty<GetItemTypeCategoryTreeResult>();

    public IReadOnlyList<string> ItemNames { get; set; } = Array.Empty<string>();
}