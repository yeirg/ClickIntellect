using CI.Api.Application.Events;
using CI.Api.Infrastructure;
using CI.Api.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace CI.Api.Application.EventHandlers;

public class ProductsFileUploadedEventHandler(
    ProductsDbContext dbContext, 
    ProductParserFactory factory,
    IMemoryCache cache)
    : INotificationHandler<ProductsFileUploadedEvent>
{
    public Task Handle(ProductsFileUploadedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var parser = factory.Parser(Path.GetExtension(notification.File.FileName));
            var products = parser.Parse(notification.File);
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();
            cache.Set(notification.Id.ToString(), FileStatus.Complete);
        }
        catch (Exception)
        {
            cache.Set(notification.Id.ToString(), FileStatus.Error);
        }
        
        return Task.CompletedTask;
    }
}