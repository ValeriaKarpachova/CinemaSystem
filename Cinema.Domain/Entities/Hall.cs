using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities;

public partial class Hall
{
    public int HallId { get; set; }

    public int Capacity { get; set; }

    public string ScreenType { get; set; } = null!;

    public string HallName { get; set; } = null!;

    public int RowsCount { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
