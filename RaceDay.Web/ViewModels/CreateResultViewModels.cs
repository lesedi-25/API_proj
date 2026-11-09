using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateResultViewModel
    {
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