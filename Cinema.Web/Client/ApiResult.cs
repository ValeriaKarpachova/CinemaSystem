namespace Cinema.Web.Client;

public class ApiResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public bool IsConflict => StatusCode == 409;
    public bool IsUnauthorized => StatusCode == 401;
}

public class ApiResult<T> : ApiResult
{
    public T? Value { get; init; }
}