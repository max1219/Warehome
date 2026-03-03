namespace Warehome.Application.DTO.Output.ItemStock;

public class GetAllStocksByTypeResult
{
    public GetAllStocksByTypeStatus Status { get; set; }
    public IEnumerable<GetItemStockResult>? Result { get; set; }
}