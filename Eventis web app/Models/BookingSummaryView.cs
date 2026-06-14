using System;
using System.ComponentModel.DataAnnotations;

namespace Eventis_web_app.Models;

public class BookingSummaryView
{
    public Guid BookingId { get; set; }
    
    [Display(Name = "Booking Date")]
    public DateTime BookingDate { get; set; }
    
    public string? Status { get; set; }
    public int VenueId { get; set; }
    
    [Display(Name = "Venue Name")]
    public string VenueName { get; set; } = null!;
    
    public string? Location { get; set; }
    public int Capacity { get; set; }
    public int EventId { get; set; }
    
    [Display(Name = "Event Name")]
    public string EventName { get; set; } = null!;
    
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }
    
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    public bool IsAvailable { get; set; }
    public int? EventTypeId { get; set; }
    
    [Display(Name = "Event Type")]
    public string? EventTypeName { get; set; }
}
