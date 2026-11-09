
using System;
using System.Text.Json.Serialization;

namespace RaceDay.Web.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }

        public string? EventName { get; set; }

        [JsonPropertyName("eventDescription")]
        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        public string? Location { get; set; }

        public decimal DistanceKm { get; set; }

        public string? EventType { get; set; }

        public decimal EntryFee { get; set; }

        public int OrganiserId { get; set; }

        // Local event banner image path.
        [JsonIgnore]
        public string? EventBannerPath { get; set; }
    }
}
