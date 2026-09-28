using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities;

public partial class Seat
{
    public int SeatId { get; set; }

    public int HallId { get; set; }

    public int RowNumber { get; set; }

    public int SeatNumber { get; set; }

    public decimal Price { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
