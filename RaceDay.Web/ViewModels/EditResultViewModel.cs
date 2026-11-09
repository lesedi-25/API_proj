using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class EditResultViewModel
    {
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        public TimeSpan? FinishTime { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Position must be greater than zero.")]
        public int? Position { get; set; }

        [Required]
        public string Status { get; set; } = "Finished";
    }
}