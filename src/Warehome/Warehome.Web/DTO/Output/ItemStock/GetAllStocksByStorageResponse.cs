namespace Warehome.Web.DTO.Output;

public class GetAllStocksByStorageResponse
{
    public required GetAllStocksByStorageStatus Status { get; init; }
    public IEnumerable<GetItemStockResponse>? Result { get; init; }
}