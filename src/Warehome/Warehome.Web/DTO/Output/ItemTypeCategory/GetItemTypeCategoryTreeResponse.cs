namespace Warehome.Web.DTO.Output;

public class GetItemTypeCategoryTreeResponse
{
    public string Name { get; set; }
    public IReadOnlyList<GetItemTypeCategoryTreeResponse> Children { get; set; }
    public int ChildCount { get; set; }
    public IReadOnlyList<string> ItemNames { get; set; }
    public int ItemTypeCount { get; set; }
}