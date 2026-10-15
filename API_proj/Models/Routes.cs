
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models
{
    public class Routes
    {
        [Key]
        public int RouteId { get; set; }

        [Required]
        public int EventId { get; set; }

        public Events? Event { get; set; }

        [Required]
        [StringLength(100)]
        public string RouteName { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        [StringLength(1000)]
        public string RouteDescription { get; set; } = string.Empty;

        [StringLength(150)]
        public string StartLocation { get; set; } = string.Empty;

        [StringLength(150)]
        public string EndLocation { get; set; } = string.Empty;
    }
}
