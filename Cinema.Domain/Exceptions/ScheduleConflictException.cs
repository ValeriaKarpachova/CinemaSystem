namespace Cinema.Domain.Exceptions;

public class ScheduleConflictException : Exception
{
    public ScheduleConflictException(Exception? inner = null)
        : base("Schedule conflict", inner) { }
}