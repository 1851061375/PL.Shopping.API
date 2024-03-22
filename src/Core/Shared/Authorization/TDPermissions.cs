using System.Collections.ObjectModel;

namespace TD.WebApi.Shared.Authorization;

public static class TDAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
    public const string Manage = nameof(Manage);
}

public static class TDResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string Students = nameof(Students);
    public const string Distributors = nameof(Distributors);
    public const string System = nameof(System);
    public const string Email = nameof(Email);
    public const string Resource = nameof(Resource);

    public const string CommonCategories = nameof(CommonCategories);
    public const string Permissions = nameof(Permissions);
    public const string Portal = nameof(Portal);
    public const string Reports = nameof(Reports);
    public const string ReportPayment = nameof(ReportPayment);
    public const string TokuteiOrders = nameof(TokuteiOrders);

}

public static class TDSection
{
    #region common
    public const string Tenants = nameof(Tenants);
    public const string System = "Hệ thống";
    public const string Users = "Người dùng";
    public const string Portal = "Cổng thông tin";
    public const string CommonCategories = "Danh mục chung";
    public const string Reports = "Báo cáo thống kê";
    public const string TokuteiOrders = "Đơn hàng tokutei";
    #endregion
}

public static class TDPermissions
{
    private static readonly TDPermission[] _all =
    [
        new("Xóa người dùng", TDAction.Delete, TDResource.Users, TDSection.Users),
        new("Quản trị tài khoản người dùng/giáo viên", TDAction.Manage, TDResource.Users, TDSection.Users),
        new("Quản trị tài khoản học sinh/phụ huynh", TDAction.Manage, TDResource.Students, TDSection.Users),
        new("Quản trị tài khoản cộng tác viên/đại lý", TDAction.Manage, TDResource.Distributors, TDSection.Users),
        new("Quản trị phân quyền người dùng", TDAction.Manage, TDResource.Permissions, TDSection.Users),

        new("Quản trị danh mục chung", TDAction.Manage, TDResource.CommonCategories, TDSection.CommonCategories),

        new("Quản trị cấu hình hệ thống", TDAction.Manage, TDResource.System, TDSection.System),
        new("Quản trị cấu hình Email", TDAction.Manage, TDResource.Email, TDSection.System),

        new("Quản trị cổng thông tin", TDAction.Manage, TDResource.Portal, TDSection.Portal),
        new("Theo dõi báo cáo thống kê chung", TDAction.Manage, TDResource.Reports, TDSection.Reports),
        new("Quản trị báo cáo thống kê doanh thu", TDAction.Manage, TDResource.ReportPayment, TDSection.Reports),

        new("Quản trị đơn hàng", TDAction.Manage, TDResource.TokuteiOrders, TDSection.TokuteiOrders),
    ];

    public static IReadOnlyList<TDPermission> All { get; } = new ReadOnlyCollection<TDPermission>(_all);
    public static IReadOnlyList<TDPermission> Root { get; } = new ReadOnlyCollection<TDPermission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<TDPermission> Admin { get; } = new ReadOnlyCollection<TDPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<TDPermission> Basic { get; } = new ReadOnlyCollection<TDPermission>(_all.Where(p => p.IsBasic).ToArray());
}

public record TDPermission(string Description, string Action, string Resource, string Section = TDSection.System, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
