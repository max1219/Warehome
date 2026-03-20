namespace Warehome.Domain.Entities;

public class ItemStock
{
    public int Id { get; set; }
    public ItemType ItemType { get; set; }
    public Storage Storage { get; set; }
    public int Quantity { get; set; }
}