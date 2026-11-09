
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class RouteCreateViewModel
    {
        [Required]
        public string RouteName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.1, 1000)]
        public double Distance { get; set; }

        [Required]
        public string StartLocation { get; set; } = string.Empty;

        [Required]
        public string EndLocation { get; set; } = string.Empty;

        public string RouteMapUrl { get; set; } = string.Empty;
    }
}
