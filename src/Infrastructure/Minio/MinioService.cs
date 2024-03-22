using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.Minio;
using TD.WebApi.Infrastructure.Common.Extensions;

namespace TD.WebApi.Infrastructure.Minio;

public class MinioService : IMinioService
{
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioService> _logger;
    private readonly IStringLocalizer _t;
    private readonly IMinioClient _minioClient;

    public MinioService(IOptions<MinioSettings> settings, ILogger<MinioService> logger, IStringLocalizer<MinioService> localizer)
    {
        _t = localizer;
        _settings = settings.Value;
        _logger = logger;
        _minioClient = new MinioClient()
                                    .WithEndpoint(settings.Value.Endpoint)
                             .WithCredentials(settings.Value.AccessKey, settings.Value.SecretKey)
                             .WithSSL().Build()
                             ;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
    {
        var beArgs = new BucketExistsArgs()
                   .WithBucket(bucketName);

        bool bucketExists = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!bucketExists)
        {
            var mbArgs = new MakeBucketArgs()
                       .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        var fileStream = file.OpenReadStream();

        string? fileName = file.FileName.Trim('"');
        fileName = fileName.RemoveSpecialCharacters(string.Empty);
        fileName = fileName.ReplaceWhitespace("_");

        var request = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(string.IsNullOrEmpty(prefix) ? fileName : $"{prefix?.TrimEnd('/')}/{fileName}")
            .WithStreamData(fileStream)
            .WithContentType(file.ContentType)
            .WithObjectSize(fileStream.Length)
            ;
        await _minioClient.PutObjectAsync(request).ConfigureAwait(false);

        string result = bucketName + "/";
        result += string.IsNullOrEmpty(prefix) ? fileName : $"{prefix?.TrimEnd('/')}/{fileName}";

        return result;
    }

    public async Task<MemoryStream> GetFileByKeyAsync(string bucketName, string key)
    {
        var beArgs = new BucketExistsArgs()
                  .WithBucket(bucketName);

        bool bucketExists = await _minioClient.BucketExistsAsync(beArgs);
        if (!bucketExists)
        {
            throw new InternalServerException(string.Format(_t["Khong ton tai {0}."], bucketName));
        }
        var memory = new MemoryStream();

        try
        {
            StatObjectArgs statObjectArgs = new StatObjectArgs()
                                       .WithBucket(bucketName)
                                       .WithObject(key);
            await _minioClient.StatObjectAsync(statObjectArgs);

            GetObjectArgs getObjectArgs = new GetObjectArgs()
                                     .WithBucket(bucketName)
                                     .WithObject(key)
                                     .WithCallbackStream(stream =>
                                     {
                                         stream.CopyTo(memory);
                                         stream.Dispose();
                                     })
                                    ;

            var file = await _minioClient.GetObjectAsync(getObjectArgs);
        }
        catch (Exception e)
        {
            throw new InternalServerException(string.Format(_t["Loi {0}."], bucketName));
        }

        return memory;
    }

    public async Task<List<AttachmentResponse>> UploadFilesAsync(List<IFormFile> files, string? bucketName, string? prefix, CancellationToken cancellationToken = default)
    {
        List<AttachmentResponse> listFile = new List<AttachmentResponse>();
        if (files.Any(f => f.Length == 0))
        {
            throw new InvalidOperationException("File Not Found.");
        }

        if (string.IsNullOrEmpty(bucketName))
        {
            bucketName = _settings.BucketName;
        }

        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                string? fileName = formFile.FileName.Trim('"');
                fileName = fileName.RemoveSpecialCharacters(string.Empty);
                fileName = fileName.ReplaceWhitespace("_");

                var attachment = new AttachmentResponse();
                attachment.Name = fileName;
                attachment.Type = Path.GetExtension(formFile.FileName);
                attachment.Url = await UploadFileAsync(formFile, bucketName!, prefix ?? Guid.NewGuid().ToString());
                attachment.Size = formFile.Length;

                listFile.Add(attachment);
            }
        }

        return await Task.FromResult(listFile);

    }

    public Task<AttachmentResponse> UploadFileAsync<T>(IFormFile? file, string? bucketName, string? prefix, CancellationToken cancellationToken = default)
        where T : class
    {
        throw new NotImplementedException();
    }

}