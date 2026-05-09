using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eventis_web_app.Models;

[Index("VenueId", "BookingDate", Name = "UQ_Venue_Date", IsUnique = true)]
public partial class Booking
{
    [Key]
    [Column("BookingID")]
    public Guid BookingId { get; set; }

    [Required(ErrorMessage = "Please select an event.")]
    [Column("EventID")]
    [Display(Name = "Event")]
    public int EventId { get; set; }

    [Required(ErrorMessage = "Please select a venue.")]
    [Column("VenueID")]
    [Display(Name = "Venue")]
    public int VenueId { get; set; }

    [Required(ErrorMessage = "Booking date is required.")]
    [Column(TypeName = "datetime")]
    [Display(Name = "Booking Date")]
    public DateTime? BookingDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("Bookings")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("VenueId")]
    [InverseProperty("Bookings")]
    public virtual Venue Venue { get; set; } = null!;
}
