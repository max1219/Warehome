namespace Warehome.Domain.Entities;

public class Storage
{
    public string Name { get; set; }
    public Category<Storage>? Category { get; set; }
    public List<StockItem> Items { get; set; }
}