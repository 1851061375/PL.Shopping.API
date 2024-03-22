using System.Security.Claims;
using TD.WebApi.Application.Identity.Permissions;
using TD.WebApi.Application.Identity.Roles;
using TD.WebApi.Application.Identity.Users.Password;

namespace TD.WebApi.Application.Identity.Users;

public interface IUserService : ITransientService
{
    Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken);
    Task<PaginationResponse<UserSearchDto>> SearchAsyncNew(UserListFilter filter, CancellationToken cancellationToken);

    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null);

    Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);
    Task<UserDetailsDto> GetCurrentUserAsync(CancellationToken cancellationToken);


    Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken);
    Task<UserDetailsDto>? GetByUsernameAsync(string userName, CancellationToken cancellationToken);

    Task<Result<UserDetailsDto>> GetByMyRefCodeAsync(string myRefCode, CancellationToken cancellationToken);

    Task<List<UserRoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken);
    Task<string> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken);

    Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken);

    Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken);
    Task ChangeTypeAsync(ChangeTypeRequest request, CancellationToken cancellationToken);

    Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);
    Task<string> CreateOtherAsync(CreateUserPublicRequest request, string origin);
    Task<string> CreateAsync(CreateUserRequest request, string origin);
    Task UpdateAsync(UpdateUserRequest request, string userId);
    Task UpdateAsync(PersonalUpdateUserRequest request, string userId);
    Task<bool> UpdateByUserIdAsync(UpdateUserRequest request, string userId);
    Task<bool> UpdateAsyncNew(UpdateUserRequest request, string userId);

    Task<string> ConfirmEmailAsync(string userId, string code, string tenant, CancellationToken cancellationToken);
    Task<string> ConfirmPhoneNumberAsync(string userId, string code);

    Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
    Task<string> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> AdminResetPasswordAsync(AdminResetPasswordRequest request);
    Task<bool> DeleteAsyncByUserId(string userId);

    Task ChangePasswordAsync(ChangePasswordRequest request, string userId);
    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken);
    Task<TfaSetupDto> GetTfaSetupAsync(string userId, CancellationToken cancellationToken);
    Task<bool> PostTfaSetupAsync(string userId, TfaSetupRequest request, CancellationToken cancellationToken);
    Task<UserPermissionDto> GetByUserNameWithPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<string> UpdatePermissionsAsync(UpdateUserPermissionsRequest request, CancellationToken cancellationToken);
    Task<string> UpdateDiscountConfigAsync(string userId, UpdateDiscountConfigRequest request, CancellationToken cancellationToken);

}