using OzonService.Application.Abstractions.Persistence;
using OzonService.Application.Abstractions.Persistence.Queries;
using OzonService.Application.Contracts.Reports;
using OzonService.Application.Models.Reports;

namespace OzonService.Application.Services.Reports;

public class ReportService : IReportService
{
    private readonly IPersistenceContext _context;

    public ReportService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<ReportResult> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        Report? report = await _context.ReportRepository
            .QueryAsync(ReportQuery.Build(r => r.WithId(id)), cancellationToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (report is null)
        {
            return new ReportResult.Failed(ReportState.Processing);
        }

        return new ReportResult.Success(ReportState.Completed, report);
    }

    public async Task RemoveReportAsync(long id, CancellationToken cancellationToken)
    {
        await _context.ReportRepository.RemoveAsync(id, cancellationToken);
    }

    public async Task AddReport(Report report, CancellationToken cancellationToken)
    {
        await _context.ReportRepository.AddAsync(report, cancellationToken);
    }

    public IAsyncEnumerable<ReportRequest> GetUnprocessedAsync(CancellationToken cancellationToken)
    {
        return _context.ReportInboxRepository.GetUnprocessedAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(long id, CancellationToken cancellationToken)
    {
        await _context.ReportInboxRepository.MarkAsProcessedAsync(id, cancellationToken);
    }
}