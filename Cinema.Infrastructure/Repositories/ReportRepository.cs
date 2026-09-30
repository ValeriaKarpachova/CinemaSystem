using Cinema.Domain.Interfaces;
using Cinema.Domain.Reports;
using Cinema.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IDbContextFactory<CinemaDbContext> _factory;

    public ReportRepository(IDbContextFactory<CinemaDbContext> factory) => _factory = factory;

    // Лістинг 3.5 (+ фільтр за період на рівні сеансу)
    public async Task<List<MovieRevenueRow>> GetMovieRevenueAsync(DateOnly from, DateOnly to)
    {
        const string sql = @"
            SELECT m.title AS Title,
                   COUNT(t.ticket_id) AS TicketsSold,
                   SUM(t.price) AS TotalRevenue,
                   ROUND(AVG(t.price), 2) AS AvgPrice
            FROM cinema_management.tickets t
            JOIN cinema_management.sessions s ON t.session_id = s.session_id
            JOIN cinema_management.movies m ON s.movie_id = m.movie_id
            WHERE t.status = 'Оплачено'
              AND s.session_date BETWEEN @From AND @To
            GROUP BY m.title
            ORDER BY TotalRevenue DESC;";

        await using var db = await _factory.CreateDbContextAsync();
        var conn = db.Database.GetDbConnection();
        var rows = await conn.QueryAsync<MovieRevenueRow>(sql, new { From = from, To = to });
        return rows.ToList();
    }

    // Лістинг 3.6
    public async Task<MovieRevenueRow?> GetTopMovieAsync(DateOnly from, DateOnly to)
    {
        const string sql = @"
            SELECT m.title AS Title, COUNT(t.ticket_id) AS TicketsSold
            FROM cinema_management.tickets t
            JOIN cinema_management.sessions s ON t.session_id = s.session_id
            JOIN cinema_management.movies m ON s.movie_id = m.movie_id
            WHERE t.status = 'Оплачено'
              AND s.session_date BETWEEN @From AND @To
            GROUP BY m.title
            ORDER BY TicketsSold DESC
            LIMIT 1;";

        await using var db = await _factory.CreateDbContextAsync();
        var conn = db.Database.GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<MovieRevenueRow>(sql, new { From = from, To = to });
    }

    // Лістинг 3.7
    public async Task<List<HallLoadRow>> GetHallLoadAsync(DateOnly from, DateOnly to)
    {
        const string sql = @"
            SELECT h.hall_name AS HallName,
                   COUNT(DISTINCT s.session_id) AS SessionsCount,
                   COUNT(t.ticket_id) AS TicketsSold
            FROM cinema_management.halls h
            LEFT JOIN cinema_management.sessions s
                   ON h.hall_id = s.hall_id AND s.session_date BETWEEN @From AND @To
            LEFT JOIN cinema_management.tickets t
                   ON t.session_id = s.session_id AND t.status = 'Оплачено'
            GROUP BY h.hall_name
            ORDER BY TicketsSold DESC;";

        await using var db = await _factory.CreateDbContextAsync();
        var conn = db.Database.GetDbConnection();
        var rows = await conn.QueryAsync<HallLoadRow>(sql, new { From = from, To = to });
        return rows.ToList();
    }

    // Лістинг 3.10
    public async Task<List<DailyRevenueRow>> GetDailyRevenueAsync(DateOnly from, DateOnly to)
    {
        const string sql = @"
            SELECT payment_date::date AS PayDay,
                   SUM(amount) AS DailyRevenue,
                   SUM(SUM(amount)) OVER (ORDER BY payment_date::date) AS CumulativeRevenue
            FROM cinema_management.payments
            WHERE payment_date::date BETWEEN @From AND @To
            GROUP BY payment_date::date
            ORDER BY PayDay;";

        await using var db = await _factory.CreateDbContextAsync();
        var conn = db.Database.GetDbConnection();
        var rows = await conn.QueryAsync<DailyRevenueRow>(sql, new { From = from, To = to });
        return rows.ToList();
    }
}