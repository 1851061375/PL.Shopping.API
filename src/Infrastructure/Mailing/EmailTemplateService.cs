using RazorEngineCore;
using System.Text;
using TD.WebApi.Application.Catalog.MailConfigurations;
using TD.WebApi.Application.Common.Mailing;
using TD.WebApi.Application.Common.Persistence;

namespace TD.WebApi.Infrastructure.Mailing;

public class EmailTemplateService : IEmailTemplateService
{

    private readonly IDapperRepository _dapperRepository;

    public EmailTemplateService(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel)
    {

        string template = GetTemplate(templateName);

        IRazorEngine razorEngine = new RazorEngine();
        IRazorEngineCompiledTemplate modifiedTemplate = razorEngine.Compile(template);

        return modifiedTemplate.Run(mailTemplateModel);
    }

    public static string GetTemplate(string templateName)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tmplFolder = Path.Combine(baseDirectory, "Email Templates");
        string filePath = Path.Combine(tmplFolder, $"{templateName}.cshtml");

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        string mailText = sr.ReadToEnd();
        sr.Close();

        return mailText;
    }

    public async Task<string> GenerateEmailTemplateAsync<T>(string templateName, T mailTemplateModel, CancellationToken cancellationToken)
    {
        string template = string.Empty;

        string sql = $"SELECT* FROM [Catalog].[MailConfigurations] WHERE IsActive = 1 AND DeletedOn IS NULL AND [Key] = '{templateName}'";
        var item = await _dapperRepository.QueryFirstOrDefaultObjectAsync<MailConfigurationDetailsDto>(sql, cancellationToken);
        if (item != null)
        {
            template = item.Content ?? string.Empty;
        }
        else
        {
            template = GetTemplate(templateName);
        }

        IRazorEngine razorEngine = new RazorEngine();
        IRazorEngineCompiledTemplate modifiedTemplate = razorEngine.Compile(template);

        return modifiedTemplate.Run(mailTemplateModel);
    }
}