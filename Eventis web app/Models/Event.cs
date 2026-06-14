using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Eventis_web_app.Models;

public partial class Event : IValidatableObject
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(255, ErrorMessage = "Event name cannot exceed 255 characters.")]
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
    [ValidateNever]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    // Self-contained framework model lifecycle verification for timeline alignment checks
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
        {
            yield return new ValidationResult(
                "The event end date cannot occur before the start date schedule.",
                new[] { nameof(EndDate) }
            );
        }
    }
}
