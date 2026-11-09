namespace RaceDay.Web.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        public int EventId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int? MinimumAge { get; set; }

        public int? MaximumAge { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}