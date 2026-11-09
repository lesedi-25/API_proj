
namespace RaceDay.Web.ViewModels
{
    public class EnrolmentDisplayViewModel
    {
        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }

        public int EventId { get; set; }

        public int CategoryId { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public string EventDescription { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        public decimal EntryFee { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryDescription { get; set; } = string.Empty;
    }
}
