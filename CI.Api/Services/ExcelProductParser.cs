using CI.Domain;
using ExcelDataReader; // MIT license

namespace CI.Api.Services;

public class ExcelProductParser : IProductParser
{
    public List<Product> Parse(IFormFile content)
    {
        var list = new List<Product>();
        
        using var stream = content.OpenReadStream();
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataset = reader.AsDataSet();
        var table = dataset.Tables[0];
        
        for (var i = 1; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            var name = row[0].ToString();
            var unit = row[1].ToString();
            
            // naive parsing
#pragma warning disable CS8604
            var price = decimal.Parse(row[2].ToString());
            var quantity = int.Parse(row[3].ToString());

            var product = new Product(name, price, unit, quantity);
            
            list.Add(product);
        }
        
        return list;
    }
}