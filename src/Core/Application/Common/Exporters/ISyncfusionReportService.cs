using TD.WebApi.Shared.Common;

namespace TD.WebApi.Application.Common.Exporters;

public interface ISyncfusionReportService : ITransientService
{
    Stream GetWordFileReport(string? templateInputPath, byte[]? templateInputFile, object data, WordType type = WordType.Docx);
}