namespace Warehome.Web.Dto.Output;

public class GetStorageCategoryTreeResponse
{
    public string Name { get; set; }
    public IReadOnlyList<GetStorageCategoryTreeResponse> Children { get; set; }
    public int ChildCount { get; set; }
    public IReadOnlyList<string> StorageNames { get; set; }
    public int StorageCount { get; set; }
}