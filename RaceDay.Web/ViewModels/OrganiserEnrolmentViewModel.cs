
using System;

namespace RaceDay.Web.ViewModels
{
    public class OrganiserEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }

        public int EventId { get; set; }

        public int CategoryId { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string ParticipantName { get; set; } = string.Empty;

        public string ParticipantEmail { get; set; } = string.Empty;
    }
}
