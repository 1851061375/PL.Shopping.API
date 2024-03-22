using TD.WebApi.Application.Identity.Permissions;
namespace TD.WebApi.Host.Controllers.Identity;

public sealed class PermissionsController : VersionNeutralApiController
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService) => _permissionService = permissionService;

    [HttpGet]
   // [MustHavePermission(TDAction.View, TDResource.Roles)]
    [OpenApiOperation("Get a list of all permissions.", "")]
    public List<PermissionDto> GetAllPermissions()
    {
        return _permissionService.GetAllPermissions();
    }

    [HttpGet("group")]
   // [MustHavePermission(TDAction.View, TDResource.Roles)]
    [OpenApiOperation("Get a list of all permission groups.", "")]
    public List<PermissionGroupDto> GetAllPermissionGroups()
    {
        return _permissionService.GetAllPermissionGroups();
    }

}