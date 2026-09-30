namespace Cinema.Domain.Reports;

public class ReportSummary
{
    public decimal TotalRevenue { get; set; }
    public int TicketsSold { get; set; }
    public decimal AvgTicketPrice { get; set; }
    public string? TopMovieTitle { get; set; }
    public int TopMovieTickets { get; set; }
}