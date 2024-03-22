using TD.WebApi.Application;
using TD.WebApi.Host.Configurations;
using TD.WebApi.Host.Controllers;
using TD.WebApi.Infrastructure;
using TD.WebApi.Infrastructure.Common;
using TD.WebApi.Infrastructure.Logging.Serilog;
using Serilog;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

[assembly: ApiConventionType(typeof(TDApiConventions))]

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations().RegisterSerilog();
    builder.Services.AddControllers();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddSpaStaticFiles(c =>
    {
        c.RootPath = "ClientApp";
    });

    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "Configurations", "firebase_admin_sdk.json")),
    });

    var app = builder.Build();
    await app.Services.InitializeDatabasesAsync();
    app.UseInfrastructure(builder.Configuration);
    app.UseStaticFiles();
    app.UseSpaStaticFiles();
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp";
    });
    app.MapEndpoints();
    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}