using Grpc.Core;
using OzonService.Application.Contracts.Reports;
using OzonService.Presentation.Grpc.Extensions;
using Reports.Presentation.Grpc;

namespace OzonService.Presentation.Grpc.Controllers;

public class ReportController : ReportService.ReportServiceBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        ReportResult result = await _reportService.GetByIdAsync(request.RegistrationId, context.CancellationToken);

        return result switch
        {
            ReportResult.Success res => new GetReportResponse
            {
                RegistrationId = request.RegistrationId,
                ReportStatus = res.ReportState.ToGrpcStatus(),
                Report = res.Report.ToGrpcReport(),
            },
            ReportResult.Failed res => new GetReportResponse
            {
                RegistrationId = request.RegistrationId,
                ReportStatus = res.State.ToGrpcStatus(),
            },
            _ => throw new InvalidOperationException("Unexpected ReportResult type"),
        };
    }
}