namespace Cinema.Domain.Reports;

public class HallLoadRow
{
    public string HallName { get; set; } = "";
    public int SessionsCount { get; set; }
    public int TicketsSold { get; set; }
}