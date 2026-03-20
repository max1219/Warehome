namespace Warehome.Application.DTO.Output;

public class GetStorageCategoryTreeResult
{
    public required string Name { get; init; }

    public IReadOnlyList<GetStorageCategoryTreeResult> Children { get; set; } =
        Array.Empty<GetStorageCategoryTreeResult>();

    public IReadOnlyList<string> StorageNames { get; set; } = Array.Empty<string>();
}