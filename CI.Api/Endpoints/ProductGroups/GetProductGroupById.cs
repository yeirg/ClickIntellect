using CI.Api.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace CI.Api.Endpoints.ProductGroups;

sealed class Request
{
    public int ProductId { get; set; }
}
sealed record ProductGroupDetails(int Id, string Name, string Unit, decimal Price, int Quantity);

sealed class GetProductGroupById(ProductsDbContext dbContext) : Endpoint<
    Request,
    IEnumerable<ProductGroupDetails>>
{
    public override void Configure()
    {
        Get("/product-groups/{ProductId}");
        AllowAnonymous();
    }

    public override async Task<IEnumerable<ProductGroupDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        int groupId = req.ProductId;
        var group = await dbContext.ProductGroups
            .Where(g => g.Id == groupId)
            .Include(g => g.Products)
            .SelectMany(g => g.Products)
            .ToListAsync(cancellationToken: ct);

        return group
            .Select(product => new ProductGroupDetails(
                product.Id, 
                product.Name, 
                product.Unit, 
                product.Price, 
                product.Quantity))
            .ToList();
    }
}