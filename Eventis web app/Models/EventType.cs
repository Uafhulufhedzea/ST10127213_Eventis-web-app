using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eventis_web_app.Models;

public partial class EventType
{
    [Key]
    public int EventTypeId { get; set; }

    [Required]
    [StringLength(100)]
    public string TypeName { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
