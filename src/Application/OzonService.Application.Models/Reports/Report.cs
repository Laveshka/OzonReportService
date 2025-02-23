namespace OzonService.Application.Models.Reports;

public record Report(
    long RegistrationId,
    decimal Ratio,
    long PurchaseCount);