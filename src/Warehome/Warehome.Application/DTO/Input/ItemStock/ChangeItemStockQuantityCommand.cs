namespace Warehome.Application.DTO.Input;

public class ChangeItemStockQuantityCommand
{
    public required int Id { get; init; }
    public required int NewQuantity { get; init; }
}