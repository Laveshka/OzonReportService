using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OzonService.Application.Contracts.Reports;
using OzonService.Application.Models.Reports;
using Products.Presentation.Grpc;
using System.Transactions;

namespace OzonService.Application.Services.Reports;

public class ReportProcessingService : BackgroundService
{
    private readonly IReportService _reportService;
    private readonly ILogger<IReportService> _logger;
    private readonly ProductService.ProductServiceClient _productService;

    public ReportProcessingService(IReportService reportService, ILogger<IReportService> logger, ProductService.ProductServiceClient productService)
    {
        _reportService = reportService;
        _logger = logger;
        _productService = productService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            IAsyncEnumerable<ReportRequest> unprocessedReports = _reportService.GetUnprocessedAsync(stoppingToken);

            await foreach (ReportRequest reportEvent in unprocessedReports)
            {
                using var transaction = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                    TransactionScopeAsyncFlowOption.Enabled);
                try
                {
                    var messageRequest = new ProductStatsRequest
                    {
                        ProductId = reportEvent.ProductId,
                        StartDate = Timestamp.FromDateTimeOffset(reportEvent.StartingAt),
                        EndDate = Timestamp.FromDateTimeOffset(reportEvent.EndingAt),
                    };

                    ProductStatsResponse response = await _productService.GetProductStatsAsync(messageRequest, cancellationToken: stoppingToken);

                    if (response.TotalPurchaseCount == 0) throw new ArithmeticException("No active purchase");

                    decimal ratio = Convert.ToDecimal(response.TotalViewCount / response.TotalPurchaseCount);

                    var report = new Report(reportEvent.RegistrationId, ratio, response.TotalPurchaseCount);

                    await _reportService.AddReport(report, stoppingToken);
                    await _reportService.MarkAsProcessedAsync(reportEvent.RegistrationId, stoppingToken);

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing inbox event {RegistrationId}", reportEvent.RegistrationId);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}