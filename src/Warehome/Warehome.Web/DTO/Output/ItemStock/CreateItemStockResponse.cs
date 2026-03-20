namespace Warehome.Web.DTO.Output;

public class CreateItemStockResponse
{
    public int? Id { get; init; }
    public required CreateItemStockStatus Status { get; init; }
}