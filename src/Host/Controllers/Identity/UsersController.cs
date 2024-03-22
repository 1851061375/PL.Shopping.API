using TD.WebApi.Application.Identity.Permissions;
using TD.WebApi.Application.Identity.Users;
using TD.WebApi.Application.Identity.Users.Password;

namespace TD.WebApi.Host.Controllers.Identity;

public class UsersController : VersionNeutralApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpPost("search")]
    [OpenApiOperation("Danh sách người dùng.", "")]
    public Task<PaginationResponse<UserSearchDto>> SearchAsync(UserListFilter request, CancellationToken cancellationToken)
    {
        return _userService.SearchAsyncNew(request, cancellationToken);
    }

    [HttpGet]
    [MustHavePermission(TDAction.View, TDResource.Users)]
    [OpenApiOperation("Get list of all users.", "")]
    public Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return _userService.GetListAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    [OpenApiOperation("Get a user's details.", "")]
    public Task<UserDetailsDto> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetAsync(id, cancellationToken);
    }

    [HttpPut("{id}")]
    [OpenApiOperation("Chỉnh sửa thông tin người dùng.", "")]
    public Task<bool> UpdateUserByUserNameAsync(string id, UpdateUserRequest request)
    {
        return _userService.UpdateByUserIdAsync(request, id);
    }

    [HttpDelete("{id}")]
    [MustHavePermission(TDAction.Delete, TDResource.Users)]
    [OpenApiOperation("Xoa người dùng.", "")]
    public async Task<bool> DeleteByUserNameAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userService.DeleteUserAsync(id.ToString(), cancellationToken);

    }

    [HttpGet("{id}/permissions")]
    [OpenApiOperation("Get role details with its permissions.", "")]
    public Task<UserPermissionDto> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetByUserNameWithPermissionsAsync(id, cancellationToken);
    }

    [HttpPut("{id}/permissions")]
    [MustHavePermission(TDAction.Manage, TDResource.Permissions)]
    [OpenApiOperation("Update a role's permissions.", "")]
    public async Task<ActionResult<string>> UpdatePermissionsAsync(string id, UpdateUserPermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        return Ok(await _userService.UpdatePermissionsAsync(request, cancellationToken));
    }


    [HttpGet("{id}/roles")]
    [MustHavePermission(TDAction.Manage, TDResource.Permissions)]

    [OpenApiOperation("Get a user's roles.", "")]
    public Task<List<UserRoleDto>> GetRolesAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetRolesAsync(id, cancellationToken);
    }

    [HttpPost("{id}/roles")]
    [MustHavePermission(TDAction.Manage, TDResource.Permissions)]

    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Register))]
    [OpenApiOperation("Update a user's assigned roles.", "")]
    public Task<string> AssignRolesAsync(string id, UserRolesRequest request, CancellationToken cancellationToken)
    {
        return _userService.AssignRolesAsync(id, request, cancellationToken);
    }


    [HttpPost("{id}/discountconfig")]
    [MustHavePermission(TDAction.Manage, TDResource.Users)]
    [OpenApiOperation("Creates a new user.", "")]
    public Task<string> UpdateDiscountConfigAsync(string id, UpdateDiscountConfigRequest request, CancellationToken cancellationToken)
    {
        return _userService.UpdateDiscountConfigAsync(id, request, cancellationToken);
    }
  

    [HttpPost]
    [MustHavePermission(TDAction.Manage, TDResource.Users)]
    [OpenApiOperation("Creates a new user.", "")]
    public Task<string> CreateAsync(CreateUserRequest request)
    {
        return _userService.CreateAsync(request, GetOriginFromRequest());
    }

    [HttpPost("self-register")]
    [AllowAnonymous]
    [OpenApiOperation("Anonymous user creates a user.", "")]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Register))]
    public Task<string> SelfRegisterAsync(CreateUserPublicRequest request)
    {
        return _userService.CreateOtherAsync(request, GetOriginFromRequest());
    }

    [HttpPost("{id}/toggle-status")]
    [MustHavePermission(TDAction.Manage, TDResource.Users)]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Register))]
    [OpenApiOperation("Toggle a user's active status.", "")]
    public async Task<ActionResult> ToggleStatusAsync(string id, ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
        {
            return BadRequest();
        }

        await _userService.ToggleStatusAsync(request, cancellationToken);
        return Ok();
    }


    [HttpPost("{id}/change-type")]
    [MustHavePermission(TDAction.Manage, TDResource.Users)]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Register))]
    [OpenApiOperation("Toggle a user's active status.", "")]
    public async Task<ActionResult> ChangeTypeAsync(string id, ChangeTypeRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
        {
            return BadRequest();
        }

        await _userService.ChangeTypeAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm email address for a user.", "")]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Search))]
    public Task<string> ConfirmEmailAsync([FromQuery] string tenant, [FromQuery] string userId, [FromQuery] string code, CancellationToken cancellationToken)
    {
        return _userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken);
    }

    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm phone number for a user.", "")]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Search))]
    public Task<string> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return _userService.ConfirmPhoneNumberAsync(userId, code);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [OpenApiOperation("Request a password reset email for a user.", "")]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Register))]
    public Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        return _userService.ForgotPasswordAsync(request, GetOriginFromRequest());
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [OpenApiOperation("Reset a user's password.", "")]
    [ApiConventionMethod(typeof(TDApiConventions), nameof(TDApiConventions.Register))]
    public Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return _userService.ResetPasswordAsync(request);
    }

    [HttpPost("admin-reset-password")]
    [OpenApiOperation("Admin Reset mật khẩu.", "")]
    public Task<bool> AdminResetPasswordAsync(AdminResetPasswordRequest request)
    {
        return _userService.AdminResetPasswordAsync(request);
    }

    private string GetOriginFromRequest() => $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
}
