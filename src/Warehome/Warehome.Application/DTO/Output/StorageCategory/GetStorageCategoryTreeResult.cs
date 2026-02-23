namespace Warehome.Application.DTO.Output;

public class GetStorageCategoryTreeResult
{
    public string Name { get; set; }
    public IReadOnlyList<GetStorageCategoryTreeResult> Children { get; set; }
    public IReadOnlyList<string> StorageNames { get; set; }
}