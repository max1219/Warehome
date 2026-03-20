namespace Warehome.Web.DTO.Input;

public class ChangeItemStockQuantityRequest
{
    public required int Id { get; init; }
    public required int NewQuantity { get; init; }
}