namespace TD.WebApi.Domain.Catalog;

/// <summary>
/// Đơn hàng Tokutei
/// </summary>
public class TokuteiOrder : AuditableEntity, IAggregateRoot
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

    public TokuteiOrder(
        string? code,
        string job,
        string? province,
        int? quantity,
        string? sex,
        int? passedQuantity,
        string? salary,
        string? caregiver,
        string? applicationDate,
        string? interviewDate,
        string? expense,
        int? sortOrder,
        bool? isActive)
    {
        Code = code;
        Job = job;
        Province = province;
        Quantity = quantity;
        Sex = sex;
        PassedQuantity = passedQuantity;
        Salary = salary;
        Caregiver = caregiver;
        ApplicationDate = applicationDate;
        InterviewDate = interviewDate;
        Expense = expense;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public TokuteiOrder Update(
        string? code,
        string job,
        string? province,
        int? quantity,
        string? sex,
        int? passedQuantity,
        string? salary,
        string? caregiver,
        string? applicationDate,
        string? interviewDate,
        string? expense,
        int? sortOrder,
        bool? isActive)
    {
        Code = code;
        Job = job;
        Province = province;
        Quantity = quantity;
        Sex = sex;
        PassedQuantity = passedQuantity;
        Salary = salary;
        Caregiver = caregiver;
        ApplicationDate = applicationDate;
        InterviewDate = interviewDate;
        Expense = expense;
        SortOrder = sortOrder;
        IsActive = isActive;
        return this;
    }
}