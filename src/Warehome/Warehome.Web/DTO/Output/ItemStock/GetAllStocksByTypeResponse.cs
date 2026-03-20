namespace Warehome.Web.DTO.Output;

public class GetAllStocksByTypeResponse
{
    public required GetAllStocksByTypeStatus Status { get; init; }
    public IEnumerable<GetItemStockResponse>? Result { get; init; }
}