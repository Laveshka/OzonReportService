using OzonService.Application.Models.Reports;
using Reports.Presentation.Grpc;
using Report = OzonService.Application.Models.Reports.Report;

namespace OzonService.Presentation.Grpc.Extensions;

public static class ReportExtensions
{
    public static Reports.Presentation.Grpc.Report ToGrpcReport(this Report report)
    {
        return new Reports.Presentation.Grpc.Report
        {
            Ratio = new Ratio
            {
                Units = Convert.ToInt64(report.Ratio),
                Nanos = Convert.ToInt32((report.Ratio - Convert.ToInt64(report.Ratio)) * 1_000_000_000),
            },
            PayCount = report.PurchaseCount,
        };
    }

    public static ReportState ToDomainState(this ReportStatus grpcStatus)
    {
        return grpcStatus switch
        {
            ReportStatus.Completed => ReportState.Completed,
            ReportStatus.Processing => ReportState.Processing,
            _ => ReportState.Processing,
        };
    }

    public static ReportStatus ToGrpcStatus(this ReportState state)
    {
        return state switch
        {
            ReportState.Completed => ReportStatus.Completed,
            ReportState.Processing => ReportStatus.Processing,
            _ => ReportStatus.Processing,
        };
    }
}