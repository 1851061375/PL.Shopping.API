namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class TokuteiOrderByCodeSpec : Specification<TokuteiOrder>, ISingleResultSpecification<TokuteiOrder>
{
    public TokuteiOrderByCodeSpec(string code) =>
        Query.Where(b => b.Code == code);
}