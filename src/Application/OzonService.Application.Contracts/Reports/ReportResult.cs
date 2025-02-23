using OzonService.Application.Models.Reports;

namespace OzonService.Application.Contracts.Reports;

public abstract record ReportResult
{
    private ReportResult() { }

    public sealed record Success(ReportState ReportState, Report Report) : ReportResult;

    public sealed record Failed(ReportState State) : ReportResult;
}