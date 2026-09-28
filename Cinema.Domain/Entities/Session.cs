using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities;

public partial class Session
{
    public int SessionId { get; set; }

    public int MovieId { get; set; }

    public int HallId { get; set; }

    public TimeOnly SessionTime { get; set; }

    public DateOnly SessionDate { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
