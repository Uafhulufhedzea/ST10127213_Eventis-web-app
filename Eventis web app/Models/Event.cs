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

    [StringLength(255)]
    [Unicode(false)]
    public string EventName { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
