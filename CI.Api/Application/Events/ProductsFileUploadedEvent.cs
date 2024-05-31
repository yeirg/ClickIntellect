using MediatR;

namespace CI.Api.Application.Events;

public class ProductsFileUploadedEvent(Guid id, IFormFile file) : INotification
{
    public IFormFile File { get; } = file;
    public Guid Id { get; } = id;
    public string Extension { get; } = Path.GetExtension(file.FileName);
}