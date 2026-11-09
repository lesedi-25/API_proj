using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class EnrolmentViewModel
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
