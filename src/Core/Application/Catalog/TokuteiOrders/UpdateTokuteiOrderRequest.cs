namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class UpdateTokuteiOrderRequest : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
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

    public class UpdateTokuteiOrderRequestValidator : CustomValidator<UpdateTokuteiOrderRequest>
    {
        public UpdateTokuteiOrderRequestValidator(IRepository<TokuteiOrder> repository, IStringLocalizer<UpdateTokuteiOrderRequestValidator> T) =>
            RuleFor(p => p.Code)
                .NotEmpty()
                .MaximumLength(256)
                .MustAsync(async (item, name, ct) =>
                        await repository.FirstOrDefaultAsync(new TokuteiOrderByCodeSpec(name), ct)
                            is not TokuteiOrder existingItem || existingItem.Id == item.Id)
                    .WithMessage((_, name) => T["TokuteiOrder {0} already Exists.", name]);
    }

    public class UpdateTokuteiOrderRequestHandler : IRequestHandler<UpdateTokuteiOrderRequest, Result<Guid>>
    {
        // Add Domain Events automatically by using IRepositoryWithEvents
        private readonly IRepositoryWithEvents<TokuteiOrder> _repository;
        private readonly IStringLocalizer _t;

        public UpdateTokuteiOrderRequestHandler(IRepositoryWithEvents<TokuteiOrder> repository, IStringLocalizer<UpdateTokuteiOrderRequestHandler> localizer) =>
            (_repository, _t) = (repository, localizer);

        public async Task<Result<Guid>> Handle(UpdateTokuteiOrderRequest request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetByIdAsync(request.Id, cancellationToken);

            _ = item
            ?? throw new NotFoundException(_t["TokuteiOrder {0} Not Found.", request.Id]);

            item.Update(
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

            await _repository.UpdateAsync(item, cancellationToken);

            return Result<Guid>.Success(request.Id);
        }
    }
}