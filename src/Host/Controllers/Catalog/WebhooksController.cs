using Microsoft.AspNetCore.StaticFiles;
using TD.WebApi.Application.Catalog.Attachments;

namespace TD.WebApi.Host.Controllers.Catalog;

public class WebhooksController : VersionedApiController
{

    [HttpGet]
    [AllowAnonymous]
    [OpenApiOperation("Response request from application.", "")]
    public IActionResult Get([FromQuery] string hub_mode, [FromQuery] string hub_challenge, [FromQuery] string hub_verify_token)
    {
        if (hub_mode == "subscribe" && hub_verify_token == "PLAP")
        {
            return Ok(hub_challenge);
        }

        return Unauthorized();
    }
}