namespace Warehome.Domain.Entities;

public class ItemCategory
{
    public string Name { get; set; }
    public ItemCategory Parent { get; set; }
}