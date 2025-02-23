using NSubstitute;
using OzonService.Application.Abstractions.Persistence;
using OzonService.Application.Abstractions.Persistence.Queries;
using OzonService.Application.Contracts.Reports;
using OzonService.Application.Models.Reports;
using OzonService.Application.Services.Reports;

namespace ApplicationTests;

/// <summary>
/// Class for testing ReportService which is responsible for all actions with reports except processing.
/// </summary>
public class ReportServiceTests
{
    private readonly IPersistenceContext _context;
    private readonly ReportService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportServiceTests"/> class.
    /// </summary>
    public ReportServiceTests()
    {
        _context = Substitute.For<IPersistenceContext>();
        _service = new ReportService(_context);
    }

    /// <summary>
    /// Gets a pair of values where first is registrationId and second is repository response Report object.
    /// </summary>
    public static TheoryData<long, Report?> ReportData => new()
    {
        { 1, new Report(1, 1, 1) },
        { 2, null },
    };

    /// <summary>
    /// Method should return result according to absence off report in repository.
    /// </summary>
    /// <param name="id"> id of the report.</param>
    /// <param name="report">report from <see cref="ReportData"/>.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Theory]
    [MemberData(nameof(ReportData))]
    public async Task GetByIdAsync_ShouldReturnCorrectResult(long id, Report? report)
    {
        CancellationToken token = CancellationToken.None;
        _context.ReportRepository.QueryAsync(Arg.Any<ReportQuery>(), token)
            .Returns(report is null
                ? AsyncEnumerable.Empty<Report>()
                : new List<Report> { report }.ToAsyncEnumerable());

        ReportResult result = await _service.GetByIdAsync(id, token);

        if (report is null)
        {
            Assert.IsType<ReportResult.Failed>(result);
            Assert.Equal(ReportState.Processing, ((ReportResult.Failed)result).State);
        }
        else
        {
            Assert.IsType<ReportResult.Success>(result);
            Assert.Equal(ReportState.Completed, ((ReportResult.Success)result).ReportState);
            Assert.Equal(report, ((ReportResult.Success)result).Report);
        }
    }

    /// <summary>
    /// Method should call remove from repository with correct argument.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task RemoveReportAsync_ShouldCallRepositoryMethod()
    {
        const long id = 1;
        CancellationToken token = CancellationToken.None;

        await _service.RemoveReportAsync(id, token);
        await _context.ReportRepository.Received(1).RemoveAsync(id, token);
    }

    /// <summary>
    /// Method should call AddReport from repository with correct argument.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task AddReport_ShouldCallRepositoryMethod()
    {
        var report = new Report(1, 1, 1);
        CancellationToken token = CancellationToken.None;

        await _service.AddReport(report, token);
        await _context.ReportRepository.Received(1).AddAsync(report, token);
    }

    /// <summary>
    /// Method should return reports from repository.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task GetUnprocessedAsync_ShouldReturnCorrectResult()
    {
        CancellationToken token = CancellationToken.None;
        var expectedReports = new List<ReportRequest> { new(DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch, 1, 1) };
        _context.ReportInboxRepository.GetUnprocessedAsync(token).Returns(expectedReports.ToAsyncEnumerable());

        IAsyncEnumerable<ReportRequest> result = _service.GetUnprocessedAsync(token);
        List<ReportRequest> actualReports = await result.ToListAsync(cancellationToken: token);

        Assert.Equal(expectedReports.Count, actualReports.Count);
        Assert.Equal(expectedReports[0].RegistrationId, actualReports[0].RegistrationId);
    }

    /// <summary>
    /// Method should send the correct call to repository.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task MarkAsProcessedAsync_ShouldCallRepositoryMethod()
    {
        const long id = 1;
        CancellationToken token = CancellationToken.None;

        await _service.MarkAsProcessedAsync(id, token);
        await _context.ReportInboxRepository.Received(1).MarkAsProcessedAsync(id, token);
    }
}