using Itmo.Dev.Platform.Kafka.Consumer;
using OzonService.Application.Abstractions.Persistence;
using OzonService.Application.Models.Reports;
using Reports.Presentation.Kafka;

namespace OzonService.Presentation.Kafka.ConsumerHandlers;

public class ReportRequestConsumer : IKafkaConsumerHandler<ReportRequestKey, ReportRequestValue>
{
    private readonly IPersistenceContext _context;

    public ReportRequestConsumer(IPersistenceContext context)
    {
        _context = context;
    }

    public async ValueTask HandleAsync(IEnumerable<IKafkaConsumerMessage<ReportRequestKey, ReportRequestValue>> messages, CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<ReportRequestKey, ReportRequestValue> message in messages)
        {
            var inboxEvent = new ReportRequest(
                message.Value.StartingAt.ToDateTimeOffset(),
                message.Value.EndingAt.ToDateTimeOffset(),
                message.Value.ProductId,
                message.Key.RegistrationId);

            await _context.ReportInboxRepository.SaveAsync(inboxEvent, cancellationToken);
        }
    }
}