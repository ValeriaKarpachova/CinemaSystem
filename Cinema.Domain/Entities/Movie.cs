using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public int GenreId { get; set; }

    public int Duration { get; set; }

    public string AgeRating { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public string? Description { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
