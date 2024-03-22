using Microsoft.AspNetCore.StaticFiles;
using TD.WebApi.Application.Catalog.Attachments;

namespace TD.WebApi.Host.Controllers.Catalog;

public class AttachmentsController : VersionedApiController
{

    [HttpPost("public")]
    [DisableRequestSizeLimit]
    [OpenApiOperation("Create new attachments with minio.", "")]
    public async Task<IActionResult> PostAttachmentPublics([FromForm(Name = "files")] List<IFormFile> files)
    {
        return Ok(await Mediator.Send(new CreateAttachmentsMinioRequest() { Files = files, BucketName = "codemath" }));
    }

    [HttpGet("{bucketName}/{*key}")]
    [DisableRequestSizeLimit]
    [AllowAnonymous]
    [OpenApiOperation("Create a new attachment with minio.", "")]
   /* [AllowAnonymous]*/
    public async Task<IActionResult> SearchAsync(string bucketName, string key)
    {
        var s3Object = await Mediator.Send(new GetAttachmentInBucketMinioRequest(bucketName, key));
        //Response.Headers.Add("X-Content-Type-Options", "nosniff");
        return File(s3Object.ToArray(), GetContentType(key));
    }

    private string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        string contentType;

        if (!provider.TryGetContentType(path, out contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}