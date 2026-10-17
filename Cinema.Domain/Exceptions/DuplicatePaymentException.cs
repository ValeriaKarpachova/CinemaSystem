namespace Cinema.Domain.Exceptions;

public class DuplicatePaymentException : Exception
{
    public DuplicatePaymentException(Exception? inner = null)
        : base("Payment for this ticket already exists", inner) { }
}