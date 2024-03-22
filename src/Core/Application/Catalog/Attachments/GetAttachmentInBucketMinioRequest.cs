using Mapster;
using Microsoft.AspNetCore.Http;
using TD.WebApi.Application.Common.Minio;

namespace TD.WebApi.Application.Catalog.Attachments;

public class GetAttachmentInBucketMinioRequest : IRequest<MemoryStream>
{
    public string BucketName { get; set; }
    public string Key { get; set; }

    public GetAttachmentInBucketMinioRequest(string bucketName, string key)
    {
        BucketName = bucketName;
        Key = key;
    }
}

public class GetAttachmentInBucketMinioRequestValidator : CustomValidator<GetAttachmentInBucketMinioRequest>
{
    public GetAttachmentInBucketMinioRequestValidator(IStringLocalizer<GetAttachmentInBucketMinioRequestValidator> localizer) =>
        RuleFor(p => p.BucketName)
            .NotEmpty()
                .WithMessage((_) => string.Format(localizer["attachment.alreadyexists"]));
}

public class GetAttachmentInBucketMinioRequestHandler : IRequestHandler<GetAttachmentInBucketMinioRequest, MemoryStream>
{
    private readonly IMinioService _file;

    public GetAttachmentInBucketMinioRequestHandler(IMinioService file) => (_file) = ( file);

    public async Task<MemoryStream> Handle(GetAttachmentInBucketMinioRequest request, CancellationToken cancellationToken)
    {
        var filename = await _file.GetFileByKeyAsync(request.BucketName, request.Key);
        return filename;

    }
}