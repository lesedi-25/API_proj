
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models
{
    public class Enrolments
    {
        [Key]
        public int EnrolmentId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        public Users? Participant { get; set; }

        [Required]
        public int EventId { get; set; }

        public Events? Event { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Categories? Category { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
    }
}
