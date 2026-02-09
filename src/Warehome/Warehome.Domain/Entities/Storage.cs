namespace Warehome.Domain.Entities;

public class Storage
{
    public string Name { get; set; }
    public List<StockItem> Items { get; set; }
}