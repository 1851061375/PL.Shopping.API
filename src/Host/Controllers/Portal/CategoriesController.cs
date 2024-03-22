using TD.WebApi.Application.Catalog.Categories;

namespace TD.WebApi.Host.Controllers.Public;

public class CategoriesController : VersionedPulbicApiController
{
    [HttpPost("search")]
    [AllowAnonymous]
    [OpenApiOperation("Danh sách danh mục.", "")]
    public Task<PaginationResponse<CategoryDto>> SearchAsync(SearchCategoriesRequest request)
    {
        request.IsActive = true;
        return Mediator.Send(request);
    }
}