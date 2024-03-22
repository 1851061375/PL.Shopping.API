using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using TD.WebApi.Application.Common.Caching;
using TD.WebApi.Application.Common.Events;
using TD.WebApi.Application.Common.Exceptions;
using TD.WebApi.Application.Common.FileStorage;
using TD.WebApi.Application.Common.Interfaces;
using TD.WebApi.Application.Common.Mailing;
using TD.WebApi.Application.Common.Models;
using TD.WebApi.Application.Common.Persistence;
using TD.WebApi.Application.Identity.Permissions;
using TD.WebApi.Application.Identity.Users;
using TD.WebApi.Domain.Identity;
using TD.WebApi.Infrastructure.Auth;
using TD.WebApi.Infrastructure.Persistence.Context;
using TD.WebApi.Shared.Authorization;
using TD.WebApi.Shared.Utils;

namespace TD.WebApi.Infrastructure.Identity;

internal partial class UserService : IUserService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _db;
    private readonly IStringLocalizer _t;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly SecuritySettings _securitySettings;
    private readonly IEmailTemplateService _templateService;
    private readonly IFileStorageService _fileStorage;
    private readonly IEventPublisher _events;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly ITenantInfo _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IDapperRepository _repository;
    private readonly UrlEncoder _urlEncoder;
    private readonly IDapperRepository _dapperRepository;
    private readonly IPermissionService _permissionService;


    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public UserService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db,
        IStringLocalizer<UserService> localizer,
        IJobService jobService,
        IMailService mailService,
        IEmailTemplateService templateService,
        IFileStorageService fileStorage,
        IEventPublisher events,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        ITenantInfo currentTenant,
        ICurrentUser currentUser,
        IDapperRepository repository,
        IDapperRepository dapperRepository,
        UrlEncoder urlEncoder,
        IPermissionService permissionService,
        IOptions<SecuritySettings> securitySettings)
    {
        _permissionService = permissionService;
        _urlEncoder = urlEncoder;
        _repository = repository;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _t = localizer;
        _jobService = jobService;
        _mailService = mailService;
        _templateService = templateService;
        _fileStorage = fileStorage;
        _events = events;
        _cache = cache;
        _cacheKeys = cacheKeys;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
        _currentUser = currentUser;
        _dapperRepository = dapperRepository;
    }

    public async Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
    {
        var spec = new UserListFilterSpec(filter);
        var specPag = new UserListPaginationFilterSpec(filter);

        var users = await _userManager.Users
            .WithSpecification(specPag)
            .ProjectToType<UserDetailsDto>()
            .ToListAsync(cancellationToken);
        int count = await _userManager.Users
            .WithSpecification(spec)
            .CountAsync(cancellationToken);

        return new PaginationResponse<UserDetailsDto>(users, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<Result<UserDetailsDto>> GetByMyRefCodeAsync(string myRefCode, CancellationToken cancellationToken)
    {

        string sql = @"SELECT Users.* FROM [Identity].[Users] Users  ";

        var itemResult = await _dapperRepository.QueryFirstOrDefaultObjectAsync<UserDetailsDto>(
            $"{sql} WHERE Users.[MyRefCode] = '{myRefCode}' ", cancellationToken: cancellationToken);

        _ = itemResult ?? throw new NotFoundException(_t["DatasetRequires {0} Not Found.", myRefCode]);

        return Result<UserDetailsDto>.Success(itemResult);
    }

    public async Task<PaginationResponse<UserSearchDto>> SearchAsyncNew(UserListFilter filter, CancellationToken cancellationToken)
    {
        string query = @"SELECT Users.* 
        FROM [Identity].[Users] Users
        LEFT JOIN Catalog.Locations LocationProvinces ON Users.ProvinceCode = LocationProvinces.Code
        LEFT JOIN Catalog.Locations LocationDistricts ON Users.DistrictCode = LocationDistricts.Code
        LEFT JOIN Catalog.Locations LocationWards ON Users.WardCode = LocationWards.Code ";

        string where = " ";

        if (filter.IsActive.HasValue)
        {

            where += $" AND Users.IsActive = {(filter.IsActive == true ? 1 : 0)} ";
        }

        if (filter.IsVerified.HasValue)
        {

            where += $" AND Users.IsVerified = {(filter.IsVerified == true ? 1 : 0)} ";
        }

        if (filter.Type.HasValue)
        {

            where += $" AND Users.Type = {filter.Type} ";
        }

        if (!string.IsNullOrEmpty(filter.Gender))
        {

            where += $" AND Users.Gender = '{filter.Gender}' ";
        }

        if (filter.Types != null && filter.Types.Any())
        {

            where += $" AND  Users.Type IN ({TDUtils.ConvertIntArrayToStringSQLQuery(filter.Types, ",")}) ";
        }

        if (filter.NotInIds != null && filter.NotInIds.Any())
        {

            where += $" AND  Users.Id NOT IN ({TDUtils.ConvertGuidArrayToStringSQLQuery(filter.NotInIds, ",")}) ";
        }

        if (filter.FromDate.HasValue)
        {
            DateTime date = filter.FromDate.Value;

            where += $" AND CAST(CONVERT(date, Users.CreatedOn) AS datetime2) >= CAST('{date.ToString("yyyy-MM-dd")}' AS datetime2) ";
        }

        if (filter.ToDate.HasValue)
        {
            DateTime date = filter.ToDate.Value;

            where += $" AND CAST(CONVERT(date, Users.CreatedOn) AS datetime2) <= CAST('{date.ToString("yyyy-MM-dd")}' AS datetime2) ";
        }

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            where += $" AND (Users.FullName LIKE N'%{filter.Keyword}%' OR Users.Email LIKE '%{filter.Keyword}%'  OR Users.PhoneNumber LIKE '%{filter.Keyword}%'  OR Users.UserName LIKE '%{filter.Keyword}%' ) ";
        }

        where = " WHERE Users.TenantId = '@tenant' " + where;
        string whereOrder = " ORDER BY Main.CreatedOn DESC ";

        string paging = $" OFFSET {(filter.PageNumber - 1) * filter.PageSize} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY";

        string sql = $"WITH Main AS ({query} {where}), TotalCount AS (SELECT COUNT(Id) AS [TotalCount]  FROM Main) SELECT * FROM Main, TotalCount {whereOrder} {paging} ";

        return await _dapperRepository.PaginatedListNewAsync<UserSearchDto>(sql, filter.PageNumber, filter.PageSize, cancellationToken);
    }


    public async Task<bool> ExistsWithNameAsync(string name)
    {
        EnsureValidTenant();
        return await _userManager.FindByNameAsync(name) is not null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        EnsureValidTenant();
        return await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        EnsureValidTenant();
        return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is ApplicationUser user && user.Id != exceptId;
    }

    private void EnsureValidTenant()
    {
        if (string.IsNullOrWhiteSpace(_currentTenant?.Id))
        {
            throw new UnauthorizedException(_t["Invalid Tenant."]);
        }
    }

    public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken) =>
        (await _userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .Adapt<List<UserDetailsDto>>();

    public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        _userManager.Users.AsNoTracking().CountAsync(cancellationToken);

    public async Task<UserDetailsDto> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == _currentUser.GetUserId().ToString())
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        return user.Adapt<UserDetailsDto>();
    }

    public async Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        var userRoles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name!))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == TDClaims.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken));
        }

        permissions.AddRange(await _db.UserClaims
              .Where(rc => rc.UserId == user.Id && rc.ClaimType == TDClaims.Permission)
              .Select(rc => rc.ClaimValue)
              .ToListAsync(cancellationToken));


        var result = await _repository.QueryFirstOrDefaultObjectAsync<UserDetailsDto>($"SELECT Users.* FROM [Identity].[Users] Users WHERE Users.Id = '{user.Id}'");

        if (result == null)
        {
            throw new NotFoundException(_t["User Not Found."]);
        }

        result.Permissions = permissions.Distinct().ToList();

        return result;
    }




    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        bool isAdmin = await _userManager.IsInRoleAsync(user, TDRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_t["Administrators Profile's Status cannot be toggled"]);
        }

        user.IsActive = request.ActivateUser;

        await _userManager.UpdateAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    public async Task ChangeTypeAsync(ChangeTypeRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        bool isAdmin = await _userManager.IsInRoleAsync(user, TDRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_t["Administrators Profile's Status cannot be toggled"]);
        }

        user.Type = request.Type;

        await _userManager.UpdateAsync(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    public async Task<TfaSetupDto> GetTfaSetupAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        bool isTfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        string? authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (authenticatorKey == null)
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        string formattedKey = GenerateQRCode(user.UserName!, authenticatorKey!);

        return new TfaSetupDto { AuthenticatorKey = authenticatorKey, IsTfaEnabled = isTfaEnabled, FormattedKey = formattedKey };
    }

    public string GenerateQRCode(string email, string unformattedKey)
    {
        return string.Format(
        AuthenticatorUriFormat,
        _urlEncoder.Encode("TD Two-Factor Auth"),
        _urlEncoder.Encode(email),
        unformattedKey);
    }

    public async Task<bool> PostTfaSetupAsync(string userId, TfaSetupRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        _ = user ?? throw new NotFoundException(_t["User Not Found."]);

        bool isValidCode = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);
        if (isValidCode)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<UserDetailsDto>? GetByUsernameAsync(string userName, CancellationToken cancellationToken)
    {
        var result = await _repository.QueryFirstOrDefaultObjectAsync<UserDetailsDto>($"SELECT Users.* FROM [Identity].[Users] Users WHERE Users.UserName = '{userName}'", cancellationToken);

        return result;
    }


}