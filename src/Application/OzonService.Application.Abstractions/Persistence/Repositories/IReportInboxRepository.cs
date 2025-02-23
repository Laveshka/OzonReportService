using OzonService.Application.Models.Reports;

namespace OzonService.Application.Abstractions.Persistence.Repositories;

public interface IReportInboxRepository
{
    Task SaveAsync(ReportRequest reportRequest, CancellationToken cancellationToken);

    IAsyncEnumerable<ReportRequest> GetUnprocessedAsync(CancellationToken cancellationToken);

    Task MarkAsProcessedAsync(long id, CancellationToken cancellationToken);
}