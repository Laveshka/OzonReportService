namespace OzonService.Application.Models.Reports;

public record ReportRequest(
    DateTimeOffset StartingAt,
    DateTimeOffset EndingAt,
    long ProductId,
    long RegistrationId);