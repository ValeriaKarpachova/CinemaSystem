namespace Cinema.Web.Client;

/// <summary>Київський час та підписи дат українською (без залежності від ICU/культур).</summary>
public static class KyivTime
{
    private static readonly TimeZoneInfo Zone = Resolve();

    private static readonly string[] Months =
        ["січня", "лютого", "березня", "квітня", "травня", "червня",
         "липня", "серпня", "вересня", "жовтня", "листопада", "грудня"];

    private static readonly string[] Days =
        ["неділя", "понеділок", "вівторок", "середа", "четвер", "пʼятниця", "субота"];

    public static DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, Zone);
    public static DateOnly Today => DateOnly.FromDateTime(Now);

    public static bool IsUpcoming(DateOnly date, TimeOnly time) => date.ToDateTime(time) > Now;

    /// <summary>«Сьогодні», «Завтра» або «середа, 21 жовтня».</summary>
    public static string DayLabel(DateOnly date)
    {
        if (date == Today) return "Сьогодні";
        if (date == Today.AddDays(1)) return "Завтра";
        return $"{Days[(int)date.DayOfWeek]}, {date.Day} {Months[date.Month - 1]}";
    }

    /// <summary>Коротко: «сьогодні», «завтра» або «21.10».</summary>
    public static string ShortDay(DateOnly date)
    {
        if (date == Today) return "сьогодні";
        if (date == Today.AddDays(1)) return "завтра";
        return date.ToString("dd.MM");
    }

    private static TimeZoneInfo Resolve()
    {
        foreach (var id in new[] { "Europe/Kyiv", "Europe/Kiev", "FLE Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.Local;
    }
}
