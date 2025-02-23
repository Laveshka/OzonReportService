using OzonService.Application.Abstractions.Persistence;
using OzonService.Application.Abstractions.Persistence.Repositories;

namespace OzonService.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(
        IReportRepository reportRepository,
        IReportInboxRepository reportInboxRepository)
    {
        ReportRepository = reportRepository;
        ReportInboxRepository = reportInboxRepository;
    }

    public IReportRepository ReportRepository { get; }

    public IReportInboxRepository ReportInboxRepository { get; }
}