namespace TD.WebApi.Application.Catalog.TokuteiOrders;

public class TokuteiOrderDto : IDto
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
    public int? TotalCount { get; set; }
}