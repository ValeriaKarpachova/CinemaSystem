namespace Cinema.Domain.Exceptions;

public class DuplicateEmailException(Exception? inner = null)
    : Exception("Email already exists", inner);