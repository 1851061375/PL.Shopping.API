using Microsoft.AspNetCore.Http;

namespace TD.WebApi.Application.Common.Minio;

public interface IMinioService : IScopedService
{
    Task<string> UploadFileAsync(IFormFile file, string bucketName, string? prefix);
    public Task<List<AttachmentResponse>> UploadFilesAsync(List<IFormFile> files, string? bucketName, string? prefix, CancellationToken cancellationToken = default);
    public Task<AttachmentResponse> UploadFileAsync<T>(IFormFile? file, string? bucketName, string? prefix, CancellationToken cancellationToken = default)
    where T : class;
    Task<MemoryStream> GetFileByKeyAsync(string bucketName, string key);
}