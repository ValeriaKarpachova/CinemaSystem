using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.Exceptions;

public class RelatedRecordsExistException : Exception
{
    public RelatedRecordsExistException() : base("Запис використовується в інших таблицях.") { }
}
