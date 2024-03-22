using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TD.WebApi.Application.Catalog.LoginLogs;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.Persistence;
using TD.WebApi.Application.Common.ZoomMeeting;
using TD.WebApi.Application.Identity.Tokens;
using TD.WebApi.Infrastructure.Auth;
using TD.WebApi.Infrastructure.Auth.Jwt;
using TD.WebApi.Infrastructure.Ldap;
using TD.WebApi.Infrastructure.Multitenancy;
using TD.WebApi.Shared.Authorization;
using TD.WebApi.Shared.Multitenancy;

namespace TD.WebApi.Infrastructure.Identity;

internal class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer _t;
    private readonly SecuritySettings _securitySettings;
    private readonly JwtSettings _jwtSettings;
    private readonly TDTenantInfo? _currentTenant;
    private readonly LDAPSettings _ldapSettings;
    private readonly IDapperRepository _dapperRepository;
    private readonly ISender _mediator;

    public TokenService(
        ISender mediator,
        IDapperRepository dapperRepository,
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        IStringLocalizer<TokenService> localizer,
        TDTenantInfo? currentTenant,
        IOptions<SecuritySettings> securitySettings,
        IOptions<LDAPSettings> ldapSettings)
    {
        _dapperRepository = dapperRepository;
        _userManager = userManager;
        _t = localizer;
        _jwtSettings = jwtSettings.Value;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
        _ldapSettings = ldapSettings.Value;
        _mediator = mediator;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, string userAgent, CancellationToken cancellationToken)
    {
        var parser = UAParser.Parser.GetDefault();
        var clientInfo = parser.Parse(userAgent);

        // Lấy thông tin về trình duyệt và hệ điều hành
        string browserName = clientInfo.UA.Family; // Tên trình duyệt
        string operatingSystem = clientInfo.OS.Family; // Tên hệ điều hành

        ApplicationUser? user = null;
        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        }
        else if (!string.IsNullOrEmpty(request.UserName))
        {
            user = await _userManager.FindByNameAsync(request.UserName.Trim().Normalize());
        }

        if (string.IsNullOrWhiteSpace(_currentTenant?.Id) || user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(_t["User Not Active. Please contact the administrator."]);
        }

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
        {
            throw new UnauthorizedException(_t["E-Mail not confirmed."]);
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!_currentTenant.IsActive)
            {
                throw new UnauthorizedException(_t["Tenant is not Active. Please contact the Application Administrator."]);
            }

            if (DateTime.Now > _currentTenant.ValidUpto)
            {
                throw new UnauthorizedException(_t["Tenant Validity Has Expired. Please contact the Application Administrator."]);
            }
        }

        bool isTfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTfaEnabled)
        {
            try
            {
                await _mediator.Send(
                    new CreateLoginLogRequest
                    {
                        UserName = user.UserName ?? string.Empty,
                        UserId = new Guid(user.Id),
                        FullName = user.FullName ?? string.Empty,
                        Ip = ipAddress,
                        Type = null,
                        UserAgent = userAgent,
                        OperatingSystem = operatingSystem,
                        BrowserName = browserName,
                    },
                    cancellationToken);
            }
            catch (Exception)
            {

            }
            return await GenerateTokensAndUpdateUser(user, ipAddress);
        }

        return new TokenResponse(null, null, null, true, true);
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken)
    {

        ApplicationUser? user = null;
        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        }
        else if (!string.IsNullOrEmpty(request.UserName))
        {
            user = await _userManager.FindByNameAsync(request.UserName.Trim().Normalize());
        }

        if (string.IsNullOrWhiteSpace(_currentTenant?.Id) || user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(_t["User Not Active. Please contact the administrator."]);
        }

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
        {
            throw new UnauthorizedException(_t["E-Mail not confirmed."]);
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!_currentTenant.IsActive)
            {
                throw new UnauthorizedException(_t["Tenant is not Active. Please contact the Application Administrator."]);
            }

            if (DateTime.Now > _currentTenant.ValidUpto)
            {
                throw new UnauthorizedException(_t["Tenant Validity Has Expired. Please contact the Application Administrator."]);
            }
        }

        bool isTfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTfaEnabled)
        {
            return await GenerateTokensAndUpdateUser(user, ipAddress);
        }

        return new TokenResponse(null, null, null, true, true);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user is null)
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new UnauthorizedException(_t["Invalid Refresh Token."]);
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string ipAddress)
    {
        string token = GenerateJwt(user, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime, true, false);
    }

    private string GenerateJwt(ApplicationUser user, string ipAddress) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));


    public CreateZoomMeetingTokenResponse GetTokenAsync(CreateZoomMeetingTokenRequest request)
    {
        byte[] secret = Encoding.UTF8.GetBytes("N1L8g2oFw9reSmWr39Gdq79oxuA1KxW2");
        var securityKey = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);

        long iat = DateTimeOffset.Now.ToUnixTimeSeconds();
        long exp = iat + (60 * 600 * 2);
        var claims = new List<Claim>
{
    new Claim("sdkKey", "1u4vufVTsu34yDow7RrEg"),
    new Claim("mn", request.MeetingNumber),
    new Claim("role", request.Role.ToString(),ClaimValueTypes.Integer),
    new Claim("iat", iat.ToString(),ClaimValueTypes.Integer),
    new Claim("exp", exp.ToString(),ClaimValueTypes.Integer),
    new Claim("appKey", "1u4vufVTsu34yDow7RrEg"),
    new Claim("tokenExp", exp.ToString(),ClaimValueTypes.Integer)
};

        var token = new JwtSecurityToken(
    new JwtHeader(securityKey),
    new JwtPayload(claims));
        /*var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: securityKey);*/


        var tokenHandler = new JwtSecurityTokenHandler();
        string result = tokenHandler.WriteToken(token);

        return new CreateZoomMeetingTokenResponse() { Token = result };
    }

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress)
    {
        return new List<Claim>
        {
            new(TDClaims.NameIdentifier, user.Id),
            new(TDClaims.Sub, user.UserName ?? string.Empty),
            new(TDClaims.Email, user.Email ?? string.Empty),
            new(TDClaims.IpAddress, ipAddress),
            new(TDClaims.Tenant, _currentTenant!.Id),
        };
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.Now.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException(_t["Invalid Token."]);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    #region Xử lý đăng nhập LDAP
    public async Task<TokenResponse> GetTokenLDDAPAsync(LoginLdapRequest request, string ipAddress, CancellationToken cancellationToken)
    {

        var adUser = new ADUser();
        var tmp = _ldapSettings;

        var searchResults = SearchInAD(
            _ldapSettings.LDAPserver,
            _ldapSettings.Port,
            _ldapSettings.Domain,
            request.UserName,
            request.Password,
            $"{_ldapSettings.LDAPQueryBase}",
            new StringBuilder("(|")
                // Active Directory attributes
                .Append($"(sAMAccountName={request.UserName})")
                .Append($"(userPrincipalName={request.UserName})")
                .Append(")")
                .ToString(),
            SearchScope.Subtree,
            new string[]
            {
                    "objectGUID",
                    "sAMAccountName",
                    "displayName",
                    "mail",
                    "whenCreated",
                    "memberOf"
            });


        var results = searchResults.Entries.Cast<SearchResultEntry>();
        if (results.Any())
        {
            var resultsEntry = results.First();
            adUser = new ADUser()
            {
                objectGUID = new Guid((resultsEntry.Attributes["objectGUID"][0] as byte[])!),
                sAMAccountName = resultsEntry.Attributes["sAMAccountName"][0].ToString()!,
                /*mail = resultsEntry.Attributes["mail"][0].ToString()!,
                displayName = resultsEntry.Attributes["displayName"][0].ToString()!,
                whenCreated = DateTime.ParseExact(
                    resultsEntry.Attributes["whenCreated"][0].ToString()!,
                    "yyyyMMddHHmmss.0Z",
                    System.Globalization.CultureInfo.InvariantCulture
                )*/
            };


            var user = await _userManager.FindByNameAsync(adUser.sAMAccountName);

            if (user is null)
            {
                throw new UnauthorizedException(_t["Authentication Failed."]);
            }

            return await GenerateTokensAndUpdateUser(user, ipAddress);
        }
        else
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        throw new NotImplementedException();
    }

    public static SearchResponse SearchInAD(
        string ldapServer,
        int ldapPort,
        string domainForAD,
        string username,
        string password,
        string targetOU,
        string query,
        SearchScope scope,
        params string[] attributeList
        )
    {
        //string ldapServer = $"{subdomain}.{domain}.{zone}";
        //_logger.Debug($"Using LDAP server: {ldapServer}");

        // https://github.com/dotnet/runtime/issues/63759#issuecomment-1019318988
        // on Windows the authentication type is Negotiate, so there is no need to prepend
        // AD user login with domain. On other platforms at the moment only
        // Basic authentication is supported
        var authType = AuthType.Basic;
        // also can fail on non AD servers, so you might prefer
        // to just use AuthType.Basic everywhere
        if (!OperatingSystem.IsWindows())
        {
            authType = AuthType.Basic;
            username = OperatingSystem.IsWindows()
                ? username
                // this might need to be changed to your actual AD domain value
                : $"{domainForAD}\\{username}";
        }

        // depending on LDAP server, username might require some proper wrapping
        // instead(!) of prepending username with domain
        //username = $"uid={username},CN=Users,DC=subdomain,DC=domain,DC=zone";

        //var connection = new LdapConnection(ldapServer)
        var connection = new LdapConnection(
            new LdapDirectoryIdentifier(ldapServer, ldapPort)
            )
        {
            AuthType = authType,
            Credential = new(username, password)
        };
        // the default one is v2 (at least in that version), and it is unknown if v3
        // is actually needed, but at least Synology LDAP works only with v3,
        // and since our Exchange doesn't complain, let it be v3
        connection.SessionOptions.ProtocolVersion = 3;

        // this is for connecting via LDAPS (636 port). It should be working,
        // according to https://github.com/dotnet/runtime/issues/43890,
        // but it doesn't (at least with Synology DSM LDAP), although perhaps
        // for a different reason
        //connection.SessionOptions.SecureSocketLayer = true;

        connection.Bind();

        //_logger.Debug($"Searching scope: [{scope}], target: [{targetOU}], query: [{query}]");
        var request = new SearchRequest(targetOU, query, scope, attributeList);

        //var request = new SearchRequest("OU=DEMO,DC=tandan,DC=com,DC=vn", "(Cn=Demo1)", SearchScope.Subtree, attributeList);

        return (SearchResponse)connection.SendRequest(request);
    }
    #endregion
    public async Task<TokenResponse> GetTokenTFAAsync(TokenTFARequest request, string ipAddress, CancellationToken cancellationToken)
    {
        ApplicationUser? user = null;
        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        }
        else if (!string.IsNullOrEmpty(request.UserName))
        {
            user = await _userManager.FindByNameAsync(request.UserName.Trim().Normalize());
        }

        if (string.IsNullOrWhiteSpace(_currentTenant?.Id) || user is null)
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        bool validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);
        if (!validVerification)
        {
            throw new UnauthorizedException(_t["Authentication Failed."]);
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(_t["User Not Active. Please contact the administrator."]);
        }

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
        {
            throw new UnauthorizedException(_t["E-Mail not confirmed."]);
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!_currentTenant.IsActive)
            {
                throw new UnauthorizedException(_t["Tenant is not Active. Please contact the Application Administrator."]);
            }

            if (DateTime.Now > _currentTenant.ValidUpto)
            {
                throw new UnauthorizedException(_t["Tenant Validity Has Expired. Please contact the Application Administrator."]);
            }
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    public async Task<TokenResponse> GetTokenExternalLoginAsync(ExternalAuthRequest request, string ipAddress, string userAgent, CancellationToken cancellationToken)
    {
        var info = new UserLoginInfo(request.Provider, request.ProviderKey, request.Provider);
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        if (user == null)
        {
            if (!string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.FullName))
            {
                user = await _userManager.FindByEmailAsync(request.Email);
            }
            else
            {
                throw new InternalServerException(_t["Validation Errors Occurred."]);
            }

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.Email,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    ImageUrl = request.AvatarUrl,
                    CreatedOn = DateTime.Now,
                    Type = 1
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new InternalServerException(_t["Validation Errors Occurred."], result.GetErrors(_t));
                }

                await _userManager.AddToRoleAsync(user, TDRoles.Basic);
                await _userManager.AddLoginAsync(user, info);
            }
            else
            {
                await _userManager.AddLoginAsync(user, info);
            }
        }

        try
        {
            var parser = UAParser.Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);

            string browserName = clientInfo.UA.Family;
            string operatingSystem = clientInfo.OS.Family;

            await _mediator.Send(
                new CreateLoginLogRequest
                {
                    UserName = user.UserName ?? string.Empty,
                    UserId = new Guid(user.Id),
                    FullName = user.FullName ?? string.Empty,
                    Ip = ipAddress,
                    Type = null,
                    UserAgent = userAgent,
                    OperatingSystem = operatingSystem,
                    BrowserName = browserName,
                },
                cancellationToken);
        }
        catch (Exception)
        {

        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }
}