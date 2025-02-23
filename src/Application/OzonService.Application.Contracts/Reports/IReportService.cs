using OzonService.Application.Models.Reports;

namespace OzonService.Application.Contracts.Reports;

public interface IReportService
{
    Task<ReportResult> GetByIdAsync(
        long id,
        CancellationToken cancellationToken);

    Task RemoveReportAsync(long id, CancellationToken cancellationToken);

    Task AddReport(Report report, CancellationToken cancellationToken);

    IAsyncEnumerable<ReportRequest> GetUnprocessedAsync(CancellationToken cancellationToken);

    Task MarkAsProcessedAsync(long id, CancellationToken cancellationToken);
}