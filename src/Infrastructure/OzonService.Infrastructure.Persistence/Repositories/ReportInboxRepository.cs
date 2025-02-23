using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using OzonService.Application.Abstractions.Persistence.Repositories;
using OzonService.Application.Models.Reports;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace OzonService.Infrastructure.Persistence.Repositories;

public class ReportInboxRepository : IReportInboxRepository
{
    private readonly IPersistenceConnectionProvider _persistenceConnectionProvider;

    public ReportInboxRepository(IPersistenceConnectionProvider persistenceConnectionProvider)
    {
        _persistenceConnectionProvider = persistenceConnectionProvider;
    }

    public async Task SaveAsync(ReportRequest reportRequest, CancellationToken cancellationToken)
    {
        const string sql = """
                           insert into requests (registration_id, starting_at, ending_at, product_id, received_at)
                           values (:registration_id, :starting_at, :ending_at, :product_id, now())
                           on conflict (registration_id) do nothing;
                           """;

        await using IPersistenceConnection connection = await _persistenceConnectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("registration_id", reportRequest.RegistrationId)
            .AddParameter("starting_at", reportRequest.StartingAt)
            .AddParameter("ending_at", reportRequest.EndingAt)
            .AddParameter("product_id", reportRequest.ProductId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<ReportRequest> GetUnprocessedAsync(
        [EnumeratorCancellation]CancellationToken cancellationToken)
    {
        const string sql = """
                           select registration_id, starting_at, ending_at, product_id
                           from requests
                           where processed_at is null
                           limit 50;
                           """;

        await using IPersistenceConnection connection = await _persistenceConnectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ReportRequest(
                RegistrationId: reader.GetInt64("registration_id"),
                StartingAt: reader.GetDateTime("starting_at"),
                EndingAt: reader.GetDateTime("ending_at"),
                ProductId: reader.GetInt64("product_id"));
        }
    }

    public async Task MarkAsProcessedAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
                           update requests
                           set processed_at = now()
                           where registration_id = :id;
                           """;

        await using IPersistenceConnection connection = await _persistenceConnectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}