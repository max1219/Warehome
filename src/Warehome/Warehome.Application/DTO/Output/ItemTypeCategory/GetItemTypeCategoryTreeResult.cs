namespace Warehome.Application.DTO.Output;

public class GetItemTypeCategoryTreeResult
{
    public string Name { get; set; }
    public IReadOnlyList<GetItemTypeCategoryTreeResult> Children { get; set; }
    public IReadOnlyList<string> ItemNames { get; set; }
}