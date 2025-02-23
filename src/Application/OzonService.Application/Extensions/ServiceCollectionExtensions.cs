using Microsoft.Extensions.DependencyInjection;
using OzonService.Application.Contracts.Reports;
using OzonService.Application.Services.Reports;

namespace OzonService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IReportService, ReportService>();
        collection.AddHostedService<ReportProcessingService>();
        return collection;
    }
}