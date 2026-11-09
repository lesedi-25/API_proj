using System;

namespace RaceDay.Web.ViewModels
{
    public class EnrolmentApiViewModel
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
