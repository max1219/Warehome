namespace Warehome.Web.DTO.Output;

public class GetStorageCategoryTreeResponse
{
    public required string Name { get; init; }
    public required IReadOnlyList<GetStorageCategoryTreeResponse> Children { get; init; }
    public required int ChildCount { get; init; }
    public required IReadOnlyList<string> StorageNames { get; init; }
    public required int StorageCount { get; init; }
}