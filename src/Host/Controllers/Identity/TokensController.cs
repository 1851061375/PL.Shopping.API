using TD.WebApi.Application.Identity.Tokens;

namespace TD.WebApi.Host.Controllers.Identity;

public sealed class TokensController : VersionNeutralApiController
{
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService) => _tokenService = tokenService;

    [HttpPost]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public Task<TokenResponse> GetTokenAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        return _tokenService.GetTokenAsync(request, GetIpAddress()!, GetUserAgent()!, cancellationToken);
    }

    [HttpPost("externaltoken")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public Task<TokenResponse> GetExternalTokenAsync(ExternalAuthRequest request, CancellationToken cancellationToken)
    {
        return _tokenService.GetTokenExternalLoginAsync(request, GetIpAddress()!, GetUserAgent()!, cancellationToken);
    }

    [HttpPost("tfalogin")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public Task<TokenResponse> GetTFATokenAsync(TokenTFARequest request, CancellationToken cancellationToken)
    {
        return _tokenService.GetTokenTFAAsync(request, GetIpAddress()!, cancellationToken);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using a refresh token.", "")]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Search))]
    public Task<TokenResponse> RefreshAsync(RefreshTokenRequest request)
    {
        return _tokenService.RefreshTokenAsync(request, GetIpAddress()!);
    }

    private string? GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";

    private string? GetUserAgent() => Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"] : "N/A";
}