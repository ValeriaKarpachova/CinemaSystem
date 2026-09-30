using Cinema.Domain.Reports;
namespace Cinema.Domain.Interfaces;

public interface IReportRepository
{
    Task<List<MovieRevenueRow>> GetMovieRevenueAsync(DateOnly from, DateOnly to);
    Task<MovieRevenueRow?> GetTopMovieAsync(DateOnly from, DateOnly to);
    Task<List<HallLoadRow>> GetHallLoadAsync(DateOnly from, DateOnly to);
    Task<List<DailyRevenueRow>> GetDailyRevenueAsync(DateOnly from, DateOnly to);
}