using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TD.WebApi.Domain.Catalog;

namespace TD.WebApi.Infrastructure.Persistence.Configuration;


public class AttachmentConfig : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Name).HasMaxLength(256);
        builder.Property(b => b.Extension).HasMaxLength(128);
        builder.Property(b => b.Description).HasMaxLength(512);
        builder.Property(b => b.FileURL).HasMaxLength(512);
        builder.HasIndex(b => b.Name);
    }
}

public class AppConfigConfig : IEntityTypeConfiguration<AppConfig>
{
    public void Configure(EntityTypeBuilder<AppConfig> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Key).HasMaxLength(256);
        builder.Property(b => b.Value).HasMaxLength(256);
        builder.Property(b => b.Description).HasMaxLength(512);
        builder.HasIndex(b => b.Key);
    }
}

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Name).HasMaxLength(512);
        builder.Property(b => b.Code).HasMaxLength(256);
        builder.HasIndex(b => b.CategoryGroupId);
    }
}


public class CategoryGroupConfig : IEntityTypeConfiguration<CategoryGroup>
{
    public void Configure(EntityTypeBuilder<CategoryGroup> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Name).HasMaxLength(256);
        builder.Property(b => b.Code).HasMaxLength(256);
        builder.HasIndex(b => b.Code);
    }
}


public class LoginLogConfig : IEntityTypeConfiguration<LoginLog>
{
    public void Configure(EntityTypeBuilder<LoginLog> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.FullName).HasMaxLength(256);
        builder.Property(b => b.UserName).HasMaxLength(256);
        builder.Property(b => b.Ip).HasMaxLength(256);
        builder.Property(b => b.Type).HasMaxLength(256);
        builder.Property(b => b.UserAgent).HasMaxLength(512);
    }
}

public class MailConfigurationConfig : IEntityTypeConfiguration<MailConfiguration>
{
    public void Configure(EntityTypeBuilder<MailConfiguration> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Name).HasMaxLength(256);
        builder.Property(b => b.Key).HasMaxLength(256);
        builder.Property(b => b.Subject).HasMaxLength(512);
        builder.HasIndex(b => b.Key);

    }
}


public class NotificationConfig : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Title).HasMaxLength(256);
        builder.Property(b => b.Topic).HasMaxLength(256);
        builder.Property(b => b.Code).HasMaxLength(256);
        builder.Property(b => b.Link).HasMaxLength(256);
        builder.Property(b => b.DeepLink).HasMaxLength(512);
    }
}

public class TokuteiOrderConfig : IEntityTypeConfiguration<TokuteiOrder>
{
    public void Configure(EntityTypeBuilder<TokuteiOrder> builder)
    {
        builder.IsMultiTenant();
        builder.Property(b => b.Code).HasMaxLength(10);
        builder.Property(b => b.Job).HasMaxLength(256);
        builder.Property(b => b.Sex).HasMaxLength(10);
        builder.Property(b => b.Salary).HasMaxLength(256);
        builder.Property(b => b.Caregiver).HasMaxLength(256);
        builder.Property(b => b.ApplicationDate).HasMaxLength(256);
        builder.Property(b => b.InterviewDate).HasMaxLength(256);
        builder.Property(b => b.Expense).HasMaxLength(256);
    }
}

