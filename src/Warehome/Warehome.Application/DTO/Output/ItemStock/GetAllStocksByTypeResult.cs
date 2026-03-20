namespace Warehome.Application.DTO.Output;

public class GetAllStocksByTypeResult
{
    public required GetAllStocksByTypeStatus Status { get; init; }
    public IEnumerable<GetItemStockResult>? Result { get; init; }
}