namespace CI.Domain;

public sealed class Product : EntityBase
{
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Unit { get; private set; } = null!;
    
    /// <summary>
    /// Quantity as property vs Populated entry.
    /// Quantity as property in this case.
    /// </summary>
    public int Quantity { get; private set; }

    public bool IsProcessed { get; private set; }
    
    public decimal TotalPrice => Price * Quantity;
    
    /// <summary>
    /// Sets the product as processed.
    /// </summary>
    public void MarkAsProcessed()
    {
        IsProcessed = true;
        //RaiseDomainEvent();
        //...
    }

    public Product Split(int quantity)
    {
        //if (quantity > Quantity)
        //  throw new InvalidOperationException("Quantity to split is greater than the current quantity.");    
        var result = new Product(Name, Price, Unit, quantity);
        result.MarkAsProcessed();
        
        return result;
    }

    public Product(string name, decimal price, string unit, int quantity)
    {
        Name = name;
        Price = price;
        Unit = unit;
        Quantity = quantity;
    }
    
    /// <summary>
    /// EF Core constructor.
    /// </summary>
    private Product()
    {
        
    }   
}