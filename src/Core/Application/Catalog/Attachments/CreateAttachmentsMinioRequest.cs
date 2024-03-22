using Microsoft.AspNetCore.Http;
using TD.WebApi.Application.Common.Minio;

namespace TD.WebApi.Application.Catalog.Attachments;

public class CreateAttachmentsMinioRequest : IRequest<Result<List<AttachmentResponse>>>
{
    public List<IFormFile> Files { get; set; } = default!;
    public string? BucketName { get; set; }
    public string? Prefix { get; set; }
}

public class CreateAttachmentsMinioRequestValidator : CustomValidator<CreateAttachmentsMinioRequest>
{
    public CreateAttachmentsMinioRequestValidator(IStringLocalizer<CreateAttachmentsMinioRequestValidator> localizer) =>
        RuleFor(p => p.Files)
            .NotEmpty()
                .WithMessage((_) => string.Format(localizer["attachment.alreadyexists"]));
}

public class CreateAttachmentsMinioRequestHandler : IRequestHandler<CreateAttachmentsMinioRequest, Result<List<AttachmentResponse>>>
{
    private readonly IMinioService _file;

    public CreateAttachmentsMinioRequestHandler( IMinioService file) => (_file) = (file);

    public async Task<Result<List<AttachmentResponse>>> Handle(CreateAttachmentsMinioRequest request, CancellationToken cancellationToken)
    {
        var listFile = await _file.UploadFilesAsync(request.Files, request.BucketName, request.Prefix, cancellationToken);
        
        return Result<List<AttachmentResponse>>.Success(listFile);

    }
}