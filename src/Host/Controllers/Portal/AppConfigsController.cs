using TD.WebApi.Application.Catalog.AppConfigs;

namespace TD.WebApi.Host.Controllers.Public;

public class AppConfigsController : VersionedPulbicApiController
{
    [HttpGet]
    [AllowAnonymous]
    [OpenApiOperation("Search AppConfigs using available filters.", "")]
    public Task<PaginationResponse<AppConfigDto>> SearchAsync()
    {
        SearchAppConfigsRequest request = new SearchAppConfigsRequest();
        request.PageNumber = 1;
        request.PageSize = 100;
        request.IsActivePortal = true;
        return Mediator.Send(request);
    }
 }