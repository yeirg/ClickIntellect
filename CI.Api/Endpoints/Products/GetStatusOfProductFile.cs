using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

namespace CI.Api.Endpoints.Products;

sealed class Request
{
    public required string FileId { get; set; }
}

sealed record Response(string Status);

sealed class GetStatusOfProductFile(IMemoryCache cache) : Endpoint<
    Request, 
    Results<
        Ok<Response>,
        NotFound>>
{
    public override void Configure()
    {
        Get("/files/products/{FileId}");
        AllowAnonymous();
    }
    
    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        await Task.CompletedTask; // suppress warning
        
        if (!cache.TryGetValue(req.FileId, out string? status)) 
            return TypedResults.NotFound();
        if (status != null) 
            return TypedResults.Ok(new Response(status));

        return TypedResults.Ok(new Response(FileStatus.Unknown));
    }
}