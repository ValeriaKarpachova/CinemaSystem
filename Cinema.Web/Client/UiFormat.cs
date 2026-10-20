using System.Globalization;

namespace Cinema.Web.Client;

public static class UiFormat
{
    public static string Time(TimeOnly t) => t.ToString("HH':'mm", CultureInfo.InvariantCulture);

    public static string Duration(int minutes)
    {
        var h = minutes / 60;
        var m = minutes % 60;
        return h == 0 ? $"{m} хв" : m == 0 ? $"{h} г" : $"{h} г {m} хв";
    }

    public static string Hall(string hall, string? screenType) =>
        string.IsNullOrWhiteSpace(screenType) ? hall : $"{hall} · {screenType}";

    public static string Price(decimal value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture) + " грн";

    /// <summary>Відтінок кольору заглушки постера — стабільний для кожного фільму.</summary>
    public static int PosterHue(int id) => Math.Abs(id) * 47 % 360;

    /// <summary>Повертає returnUrl лише якщо це локальний шлях сайту (захист від open redirect).</summary>
    public static string LocalUrl(string? url) =>
        url is { Length: > 0 } && url[0] == '/' && !url.StartsWith("//") && !url.StartsWith("/\\")
            ? url
            : "/";
}
