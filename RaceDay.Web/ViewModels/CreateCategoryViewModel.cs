
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0, 120)]
        public int? MinimumAge { get; set; }

        [Range(0, 120)]
        public int? MaximumAge { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
