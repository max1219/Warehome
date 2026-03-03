namespace Warehome.Application.DTO.Output.ItemStock;

public class GetAllStocksByStorageResult
{
    public GetAllStocksByStorageStatus Status { get; set; }
    public IEnumerable<GetItemStockResult>? Result { get; set; }
}