using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eventis_web_app.Models;

public partial class Event
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(255)]
    [Unicode(false)]
    [Display(Name = "Event Name")]
    public string EventName { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [Column(TypeName = "datetime")]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [Column(TypeName = "datetime")]
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    [Column("ImageURL")]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
