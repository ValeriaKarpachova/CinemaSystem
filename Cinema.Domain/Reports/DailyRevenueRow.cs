namespace Cinema.Domain.Reports;

public class DailyRevenueRow
{
    public DateOnly PayDay { get; set; }
    public decimal DailyRevenue { get; set; }
    public decimal CumulativeRevenue { get; set; }
}