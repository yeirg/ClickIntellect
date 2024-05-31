namespace CI.Domain;

public sealed class ProductGroup : EntityBase
{
    public ICollection<Product> Products { get; private set; }
    
    public ProductGroup(List<Product> products)
    {
        Products = products;
    }

    /// <summary>
    /// EF Core constructor.
    /// </summary>
    private ProductGroup()
    {
        
    }
}