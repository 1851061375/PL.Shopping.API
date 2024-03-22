namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class CreateTokuteiOrderRequest : IRequest<Result<Guid>>
{
    public string? Code { get; set; }
    public string Job { get; set; }
    public string? Province { get; set; }
    public int? Quantity { get; set; }
    public string? Sex { get; set; }
    public int? PassedQuantity { get; set; }
    public string? Salary { get; set; }
    public string? Caregiver { get; set; }
    public string? ApplicationDate { get; set; }
    public string? InterviewDate { get; set; }
    public string? Expense { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; } = true;
}

public class CreateTokuteiOrderRequestValidator : CustomValidator<CreateTokuteiOrderRequest>
{
    public CreateTokuteiOrderRequestValidator(IReadRepository<TokuteiOrder> repository, IStringLocalizer<CreateTokuteiOrderRequestValidator> T) =>
        RuleFor(p => p.Code)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (code, ct) => await repository.FirstOrDefaultAsync(new TokuteiOrderByCodeSpec(code), ct) is null)
                .WithMessage((_, code) => T["TokuteiOrder {0} already Exists.", code]);
}

public class CreateTokuteiOrderRequestHandler : IRequestHandler<CreateTokuteiOrderRequest, Result<Guid>>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<TokuteiOrder> _repository;

    public CreateTokuteiOrderRequestHandler(IRepositoryWithEvents<TokuteiOrder> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateTokuteiOrderRequest request, CancellationToken cancellationToken)
    {
        var item = new TokuteiOrder(
            request.Code,
            request.Job,
            request.Province,
            request.Quantity,
            request.Sex,
            request.PassedQuantity,
            request.Salary,
            request.Caregiver,
            request.ApplicationDate,
            request.InterviewDate,
            request.Expense,
            request.SortOrder,
            request.IsActive);

        await _repository.AddAsync(item, cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}