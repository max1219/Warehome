namespace Warehome.Application.DTO.Output;

public class GetAllStocksByTypeResult
{
    public GetAllStocksByTypeStatus Status { get; set; }
    public IEnumerable<GetItemStockResult>? Result { get; set; }
}