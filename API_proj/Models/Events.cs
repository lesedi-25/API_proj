
using System;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models
{
    public class Events
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string EventDescription { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string EventType { get; set; } = "Run";

        [Required]
        public decimal DistanceKm { get; set; }

        public decimal EntryFee { get; set; } = 0;

        public DateTime? RegistrationDeadline { get; set; }

        // The organiser who created this event
        [Required]
        public int OrganiserId { get; set; }

        public Users? Organiser { get; set; }
    }
}
