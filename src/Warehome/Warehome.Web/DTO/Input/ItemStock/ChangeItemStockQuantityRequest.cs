namespace Warehome.Web.DTO.Input;

public class ChangeItemStockQuantityRequest
{
    public int Id { get; set; }
    public int NewQuantity { get; set; }
}