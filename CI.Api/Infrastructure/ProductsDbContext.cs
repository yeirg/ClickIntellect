using CI.Domain;
using Microsoft.EntityFrameworkCore;

namespace CI.Api.Infrastructure;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductGroup> ProductGroups { get; set; }
}