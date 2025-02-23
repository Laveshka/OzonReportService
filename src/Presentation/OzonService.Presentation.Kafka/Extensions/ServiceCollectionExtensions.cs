using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzonService.Presentation.Kafka.ConsumerHandlers;
using Reports.Presentation.Kafka;

namespace OzonService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string consumerKey = "Presentation:Kafka:Consumers";

        collection.AddPlatformKafka(builder => builder
            .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
            .AddConsumer(c => c
                .WithKey<ReportRequestKey>()
                .WithValue<ReportRequestValue>()
                .WithConfiguration(configuration.GetSection($"{consumerKey}:ReportProcessing"))
                .DeserializeKeyWithProto()
                .DeserializeValueWithProto()
                .HandleWith<ReportRequestConsumer>()));

        return collection;
    }
}