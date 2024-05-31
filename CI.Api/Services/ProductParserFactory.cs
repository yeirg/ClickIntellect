namespace CI.Api.Services;

public class ProductParserFactory
{
    public IProductParser Parser(string extension)
    {
        return extension.ToLower() switch
        {
            ".xlsx" => new ExcelProductParser(),
            _ => throw new NotSupportedException("Unsupported file extension")
        };
    }
}