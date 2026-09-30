using Cinema.Domain.Reports;

namespace Cinema.Web.Services;

public interface IReportService
{
    Task<ReportSummary> GetSummaryAsync(DateOnly from, DateOnly to);
    Task<List<MovieRevenueRow>> GetMovieRevenueAsync(DateOnly from, DateOnly to);
    Task<List<HallLoadRow>> GetHallLoadAsync(DateOnly from, DateOnly to);
    Task<List<DailyRevenueRow>> GetDailyRevenueAsync(DateOnly from, DateOnly to);
}