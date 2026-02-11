using Microsoft.EntityFrameworkCore;
using Warehome.Infrastructure.Data.Entities;

namespace Warehome.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Storage> Storages => Set<Storage>();
}