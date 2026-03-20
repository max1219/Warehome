namespace Warehome.Web.DTO.Output;

public class GetAllStocksByStorageResponse
{
    public GetAllStocksByStorageStatus Status { get; set; }
    public IEnumerable<GetItemStockResponse>? Result { get; set; }
}