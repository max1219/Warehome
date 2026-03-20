namespace Warehome.Application.DTO.Input;

public class ChangeItemStockQuantityCommand
{
    public int Id { get; set; }
    public int NewQuantity { get; set; }
}