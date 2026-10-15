using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models
{
    public class Weather
    {
        [Key]
        public int WeatherId { get; set; }

        [Required]
        public int EventId { get; set; }

        public Events? Event { get; set; }

        public decimal? TemperatureC { get; set; }

        [StringLength(50)]
        public string? Conditions { get; set; }

        public decimal? WindSpeedKph { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
