namespace Warehome.Domain.Entities;

public class StockItem
{
    public ItemType ItemType { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}