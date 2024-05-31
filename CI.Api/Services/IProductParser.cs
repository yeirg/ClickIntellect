using CI.Domain;

namespace CI.Api.Services;

public interface IProductParser
{
    List<Product> Parse(IFormFile content);
}