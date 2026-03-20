namespace Warehome.Web.DTO.Output;

public class GetAllStocksByTypeResponse
{
    public GetAllStocksByTypeStatus Status { get; set; }
    public IEnumerable<GetItemStockResponse>? Result { get; set; }
}