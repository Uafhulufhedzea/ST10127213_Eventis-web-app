using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eventis_web_app.Models;

public partial class Venue
{
    [Key]
    [Column("VenueID")]
    public int VenueId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string VenueName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Location { get; set; }

    public int? Capacity { get; set; }

    [Column("ImageURL")]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Venue")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
