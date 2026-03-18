using Warehome.Domain.Entities;

using DomainItemStock = Warehome.Domain.Entities.ItemStock;
using InfrastructureItemStock = Warehome.Infrastructure.Data.Entities.ItemStock;

namespace Warehome.Infrastructure.Mappers;

public static class InfrastructureItemStockExtension
{
    public static DomainItemStock ToDomain(this InfrastructureItemStock source)
    {
        return new DomainItemStock
        {
            Id = source.Id,
            ItemType = new ItemType
            {
                Name = source.ItemType.Name,
                Category = source.ItemType.Category == null
                    ? null
                    : new Category<ItemType>
                    {
                        Path = source.ItemType.Category.Path
                    }
            },
            Storage = new Storage
            {
                Name = source.Storage.Name,
                Category = source.Storage.Category == null
                    ? null
                    : new Category<Storage>
                    {
                        Path = source.Storage.Category.Path
                    }
            },
            Quantity = source.Quantity
        };
    }
}