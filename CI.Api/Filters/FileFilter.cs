namespace CI.Api.Filters;

public sealed class FileFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var form = await context.HttpContext.Request.ReadFormAsync();
        var fileCount = form.Files.Count;
        if (fileCount != 1)
            return Results.StatusCode(400);
        
        var file = context.HttpContext.Request.Form.Files[0];
        var extension = Path.GetExtension(file.FileName);

        // read from configuration or environment
        if (extension is not ".xlsx")
            return Results.StatusCode(400);

        return await next(context);
    }
}