namespace Warehome.Domain.Entities;

public class StockItem
{
    public Item Item { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}