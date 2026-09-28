using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int SessionId { get; set; }

    public int SeatId { get; set; }

    public int CustomerId { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Payment? Payment { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual Session Session { get; set; } = null!;
}
