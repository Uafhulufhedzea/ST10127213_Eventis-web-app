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

    [Required(ErrorMessage = "Venue name is required.")]
    [StringLength(255)]
    [Unicode(false)]
    [Display(Name = "Venue Name")]
    public string VenueName { get; set; } = null!;

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Location { get; set; }

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
    public int? Capacity { get; set; }

    [Column("ImageURL")]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Venue")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
