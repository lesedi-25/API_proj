using System;

namespace RaceDay.Web.ViewModels
{
    public class OrganiserResultViewModel
    {
        public int ResultId { get; set; }

        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string Status { get; set; } = "Finished";
    }
}