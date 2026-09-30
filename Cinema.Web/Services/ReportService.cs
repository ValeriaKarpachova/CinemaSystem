using Cinema.Domain.Interfaces;
using Cinema.Domain.Reports;

namespace Cinema.Web.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;

    public ReportService(IReportRepository repo) => _repo = repo;

    public async Task<ReportSummary> GetSummaryAsync(DateOnly from, DateOnly to)
    {
        var movies = await _repo.GetMovieRevenueAsync(from, to);
        var top = await _repo.GetTopMovieAsync(from, to);

        var ticketsSold = movies.Sum(m => m.TicketsSold);
        var totalRevenue = movies.Sum(m => m.TotalRevenue);

        return new ReportSummary
        {
            TotalRevenue = totalRevenue,
            TicketsSold = ticketsSold,
            AvgTicketPrice = ticketsSold == 0 ? 0 : Math.Round(totalRevenue / ticketsSold, 2),
            TopMovieTitle = top?.Title,
            TopMovieTickets = top?.TicketsSold ?? 0
        };
    }

    public Task<List<MovieRevenueRow>> GetMovieRevenueAsync(DateOnly from, DateOnly to) =>
        _repo.GetMovieRevenueAsync(from, to);

    public Task<List<HallLoadRow>> GetHallLoadAsync(DateOnly from, DateOnly to) =>
        _repo.GetHallLoadAsync(from, to);

    public Task<List<DailyRevenueRow>> GetDailyRevenueAsync(DateOnly from, DateOnly to) =>
        _repo.GetDailyRevenueAsync(from, to);
}