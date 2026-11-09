using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class EditEventViewModel
    {
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string EventDescription { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string EventType { get; set; } = string.Empty;

        [Range(0.1, 1000)]
        public decimal DistanceKm { get; set; }

        [Range(0, 100000)]
        public decimal EntryFee { get; set; }

        public DateTime? RegistrationDeadline { get; set; }
        public IFormFile? EventBanner { get; set; }

        public string? EventBannerPath { get; set; }
    }
}
