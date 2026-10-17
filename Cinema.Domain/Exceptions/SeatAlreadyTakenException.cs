namespace Cinema.Domain.Exceptions;

public class SeatAlreadyTakenException(Exception? inner = null)
    : Exception("Seat already taken", inner);