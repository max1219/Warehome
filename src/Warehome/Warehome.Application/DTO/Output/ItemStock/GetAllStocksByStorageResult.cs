namespace Warehome.Application.DTO.Output;

public class GetAllStocksByStorageResult
{
    public required GetAllStocksByStorageStatus Status { get; init; }
    public IEnumerable<GetItemStockResult>? Result { get; init; }
}