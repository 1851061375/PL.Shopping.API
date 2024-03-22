using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TD.WebApi.Infrastructure.Persistence.Context;

namespace TD.WebApi.Infrastructure.Identity;

internal static class Startup
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = false;
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultPhoneProvider;
                    //options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultPhoneProvider;
                    options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
                })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("emailconfirmation");

        services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromMinutes(1));
        services.Configure<EmailConfirmationTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromDays(1));
        return services;
    }
}