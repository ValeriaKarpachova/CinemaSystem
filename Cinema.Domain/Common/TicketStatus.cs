namespace Cinema.Domain.Common;

public static class TicketStatus
{
    public const string Booked = "Заброньовано";
    public const string Paid = "Оплачено";
    public const string Cancelled = "Скасовано";

    public static readonly string[] All = { Booked, Paid, Cancelled };
}