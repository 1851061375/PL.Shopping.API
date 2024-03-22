using TD.WebApi.Application.Catalog.Categories;

namespace TD.WebApi.Host.Controllers.Catalog;

public class CategoriesController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("Danh sách Danh mục.", "")]
    public Task<PaginationResponse<CategoryDto>> SearchAsync(SearchCategoriesRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Chi tiết Danh mục.", "")]
    public Task<Result<CategoryDto>> GetAsync(Guid id)
    {
        return Mediator.Send(new GetCategoryRequest(id));
    }

    [HttpPost]
    [MustHavePermission(TDAction.Manage, TDResource.CommonCategories)]
    [OpenApiOperation("Tạo mới Danh mục.", "")]
    public Task<Result<Guid>> CreateAsync(CreateCategoryRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(TDAction.Manage, TDResource.CommonCategories)]
    [OpenApiOperation("Cập nhật Danh mục.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateCategoryRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(TDAction.Manage, TDResource.CommonCategories)]
    [OpenApiOperation("Xóa Danh mục.", "")]
    public Task<Result<Guid>> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteCategoryRequest(id));
    }

}