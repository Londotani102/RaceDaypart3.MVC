
namespace RaceDay.MVC.Models
{
    public class RouteViewModel
    {
        public int RouteId { get; set; }

        public string RouteName { get; set; } = string.Empty;

        public double Distance { get; set; }

        public string StartLocation { get; set; } = string.Empty;

        public string EndLocation { get; set; } = string.Empty;
    }
}
