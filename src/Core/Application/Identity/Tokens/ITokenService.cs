using TD.WebApi.Application.Common.ZoomMeeting;

namespace TD.WebApi.Application.Identity.Tokens;

public interface ITokenService : ITransientService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, string userAgent, CancellationToken cancellationToken);

    Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken);
    CreateZoomMeetingTokenResponse GetTokenAsync(CreateZoomMeetingTokenRequest request);
    Task<TokenResponse> GetTokenTFAAsync(TokenTFARequest request, string ipAddress, CancellationToken cancellationToken);
    Task<TokenResponse> GetTokenLDDAPAsync(LoginLdapRequest request, string ipAddress, CancellationToken cancellationToken);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
    Task<TokenResponse> GetTokenExternalLoginAsync(ExternalAuthRequest request, string ipAddress, string userAgent, CancellationToken cancellationToken);

}