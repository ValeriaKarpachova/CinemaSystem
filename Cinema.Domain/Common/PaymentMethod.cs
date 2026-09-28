namespace Cinema.Domain.Common;

public static class PaymentMethod
{
    public const string Cash = "Готівка";
    public const string Card = "Картка";
    public const string Online = "Онлайн";

    public static readonly string[] All = { Cash, Card, Online };
}