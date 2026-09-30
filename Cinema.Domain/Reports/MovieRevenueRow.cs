namespace Cinema.Domain.Reports;

public class MovieRevenueRow
{
    public string Title { get; set; } = "";
    public int TicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AvgPrice { get; set; }
}