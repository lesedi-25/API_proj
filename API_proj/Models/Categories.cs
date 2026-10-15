
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public int EventId { get; set; }

        public Events? Event { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public int? MinimumAge { get; set; }

        public int? MaximumAge { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
