using CI.Api.Infrastructure;
using CI.Api.Options;
using CI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CI.Api.Jobs;

public sealed class SplitProductsIntoGroups(ProductsDbContext dbContext, IOptions<WorkflowOptions> options)
{
    private readonly WorkflowOptions options = options.Value;
    
    public async Task Execute()
{
    await using var transaction = await dbContext.Database.BeginTransactionAsync();
    try
    {
        var products = await dbContext.Products.Where(p => !p.IsProcessed).ToListAsync();
        List<ProductGroup> groups = new List<ProductGroup>();
        List<Product> currentGroup = new List<Product>();
        decimal currentTotal = 0;

        foreach (var product in products)
        {
            int quantityToAdd = 0;
            while (quantityToAdd < product.Quantity)
            {
                int maxQty = (int)((options.MaxPrice - currentTotal) / product.Price);
                if (maxQty == 0)
                {
                    groups.Add(new ProductGroup(currentGroup));
                    currentGroup = new List<Product>();
                    currentTotal = 0;
                    continue;
                }

                int qtyToAdd = Math.Min(maxQty, product.Quantity - quantityToAdd);
                
                if (currentTotal + product.Price * qtyToAdd > options.MaxPrice)
                {
                    if (currentGroup.Count > 0)
                    {
                        groups.Add(new ProductGroup(currentGroup));
                        currentGroup = new List<Product>();
                        currentTotal = 0;
                    }
                }

                var splitProduct = product.Split(qtyToAdd);
                currentGroup.Add(splitProduct);
                currentTotal += product.Price * qtyToAdd;
                quantityToAdd += qtyToAdd;
            }
        }
        
        if (currentGroup.Count > 0)
        {
            groups.Add(new ProductGroup(currentGroup));
        }

        await dbContext.ProductGroups.AddRangeAsync(groups);
        await dbContext.SaveChangesAsync();
        
        dbContext.Products.RemoveRange(products);
        await dbContext.SaveChangesAsync();
        
        await transaction.CommitAsync();
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
}