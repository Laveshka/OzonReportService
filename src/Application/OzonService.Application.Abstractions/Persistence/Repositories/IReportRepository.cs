using OzonService.Application.Abstractions.Persistence.Queries;
using OzonService.Application.Models.Reports;

namespace OzonService.Application.Abstractions.Persistence.Repositories;

public interface IReportRepository
{
    IAsyncEnumerable<Report> QueryAsync(ReportQuery query, CancellationToken cancellationToken);

    Task AddAsync(Report report, CancellationToken cancellationToken);

    Task RemoveAsync(long id, CancellationToken cancellationToken);
}