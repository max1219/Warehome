namespace Warehome.Application.DTO.Output;

public class GetAllStocksByStorageResult
{
    public GetAllStocksByStorageStatus Status { get; set; }
    public IEnumerable<GetItemStockResult>? Result { get; set; }
}