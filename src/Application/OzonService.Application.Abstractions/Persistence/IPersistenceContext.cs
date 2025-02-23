using OzonService.Application.Abstractions.Persistence.Repositories;

namespace OzonService.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IReportRepository ReportRepository { get; }

    IReportInboxRepository ReportInboxRepository { get; }
}