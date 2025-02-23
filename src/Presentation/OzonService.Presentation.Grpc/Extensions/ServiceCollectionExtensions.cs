using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OzonService.Application.Models.Products;
using Products.Presentation.Grpc;

namespace OzonService.Presentation.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection collection)
    {
        collection.AddGrpc();
        collection.AddGrpcReflection();

        return collection;
    }

    public static IServiceCollection AddClientGrpc(this IServiceCollection collection)
    {
        collection.AddGrpcClient<ProductService.ProductServiceClient>((sp, o) =>
        {
            IOptions<ProductServiceOptions> options = sp.GetRequiredService<IOptions<ProductServiceOptions>>();
            o.Address = new Uri(options.Value.Url);
        });

        return collection;
    }
}