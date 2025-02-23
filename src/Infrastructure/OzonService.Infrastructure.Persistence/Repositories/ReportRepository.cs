using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using OzonService.Application.Abstractions.Persistence.Queries;
using OzonService.Application.Abstractions.Persistence.Repositories;
using OzonService.Application.Models.Reports;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace OzonService.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public ReportRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<Report> QueryAsync(
        ReportQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
                           select  report_id,
                                   ratio,
                                   purchase_count
                           from reports
                           where report_id = any(:ids);
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("ids", query.Ids);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Report(
                RegistrationId: reader.GetInt64("report_id"),
                Ratio: reader.GetDecimal("ratio"),
                PurchaseCount: reader.GetInt64("purchase_count"));
        }
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into reports (report_id, ratio, purchase_count)
                           values (:id, :ratio, :purchase_count)
                           on conflict(report_id) do nothing;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", report.RegistrationId)
            .AddParameter("ratio", report.Ratio)
            .AddParameter("purchase_count", report.PurchaseCount);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
                           delete from reports
                           where id = :id;
                           """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}