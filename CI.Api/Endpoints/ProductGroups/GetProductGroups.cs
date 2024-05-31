using CI.Api.Infrastructure;
using FastEndpoints;

namespace CI.Api.Endpoints.ProductGroups;

sealed record ProductGroupMinimal(int Id, decimal TotalPrice);

sealed class GetProductGroups(ProductsDbContext dbContext) : EndpointWithoutRequest<
    IEnumerable<ProductGroupMinimal>>
{
    public override void Configure()
    {
        Get("/product-groups");
        AllowAnonymous();
    }

    public override Task<IEnumerable<ProductGroupMinimal>> ExecuteAsync(CancellationToken ct)
    {
        // TODO: remove conversion in prod
        var groups = dbContext.ProductGroups
            .Select(g => new ProductGroupMinimal(
                g.Id, 
                // SQLite does not support decimal
                (decimal) g.Products.Sum(p => (double)p.Price * p.Quantity)))
            .ToList();
        
        return Task.FromResult(groups.AsEnumerable());
    }
}