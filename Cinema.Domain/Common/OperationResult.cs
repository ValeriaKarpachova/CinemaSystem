using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.Common;

public record OperationResult(bool Success, string? Error = null)
{
    public static OperationResult Ok() => new(true);
    public static OperationResult Fail(string error) => new(false, error);
}
