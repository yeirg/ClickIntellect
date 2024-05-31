using CI.Api.Application.Events;
using CI.Api.Filters;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

namespace CI.Api.Endpoints.Products;

sealed class UploadProductsFileRequest
{
    public IFormFile File { get; set; } = null!;
}

sealed class UploadProductsFile(IMediator mediator, IMemoryCache cache) : Endpoint<UploadProductsFileRequest, 
    Accepted>
{
    public override void Configure()
    {
        Post("/files/products");
        AllowFileUploads();
        AllowAnonymous();
        Options(builder => builder.AddEndpointFilter<FileFilter>());
    }

    public override async Task<Accepted> ExecuteAsync(UploadProductsFileRequest req, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        cache.Set(id.ToString(), FileStatus.Processing);
        await mediator.Publish(new ProductsFileUploadedEvent(id, req.File), ct);

        return TypedResults.Accepted($"/files/products/{id}");
    }
}