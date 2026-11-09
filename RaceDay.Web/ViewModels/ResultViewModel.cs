
namespace RaceDay.Web.ViewModels
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }

        public int EnrolmentId { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
