namespace Warehome.Application.DTO.Output;

public class StorageCategoryTreeResponse
{
    public string Name { get; set; }
    public int ChildrenCount { get; set; }
    public IEnumerable<StorageCategoryTreeResponse> Children { get; set; }
    public int StoragesCount { get; set; }
    public IEnumerable<string> StorageNames { get; set; }
}