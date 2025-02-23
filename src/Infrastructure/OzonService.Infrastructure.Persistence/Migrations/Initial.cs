using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace OzonService.Infrastructure.Persistence.Migrations;

[Migration(21022025, "initial")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider) =>
    """
    create table reports (
        report_id bigint primary key,
        ratio decimal(10,4) not null,
        purchase_count bigint not null
    );
    
    create table requests (
        registration_id bigint primary key,
        starting_at timestamptz not null,
        ending_at timestamptz not null,
        product_id bigint not null,
        received_at timestamptz default now(),
        processed_at timestamptz
    );
    
    create index idx_requests_unprocessed on requests (processed_at) where processed_at is null;
    """;

    protected override string GetDownSql(IServiceProvider serviceProvider) =>
    """
    drop table if exists reports cascade;
    drop table if exists requests cascade;
    """;
}