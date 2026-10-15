
using System;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models
{
    public class Results
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        public Enrolments? Enrolment { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Finished";
    }
}
