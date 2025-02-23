using Microsoft.AspNetCore.Builder;
using OzonService.Presentation.Grpc.Controllers;

namespace OzonService.Presentation.Grpc.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentationGrpc(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGrpcService<ReportController>();
            routeBuilder.MapGrpcReflectionService();
        });

        return builder;
    }
}