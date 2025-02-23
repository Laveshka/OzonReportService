using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OzonService.Application.Abstractions.Persistence;
using OzonService.Application.Abstractions.Persistence.Repositories;
using OzonService.Infrastructure.Persistence.Plugins;
using OzonService.Infrastructure.Persistence.Repositories;

namespace OzonService.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(b => b.BindConfiguration("Infrastructure:Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)
                .WithDataSourcePlugin<MappingPlugin>()));

        collection.AddScoped<IPersistenceContext, PersistenceContext>();
        collection.AddScoped<IReportRepository, ReportRepository>();
        collection.AddScoped<IReportInboxRepository, ReportInboxRepository>();

        return collection;
    }
}