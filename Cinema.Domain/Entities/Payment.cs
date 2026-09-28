using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int TicketId { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
