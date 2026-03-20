namespace Warehome.Application.DTO.Output;

public class CreateItemStockResult
{
    public int? Id { get; init; }
    public required CreateItemStockStatus Status { get; init; }
}