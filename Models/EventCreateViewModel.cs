using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceDay.MVC.Models
{
    public class EventCreateViewModel
    {
        [Required(ErrorMessage = "Event name is required.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        // Compatibility with existing code using lowercase description
        [JsonIgnore]
        public string description
        {
            get { return Description; }
            set { Description = value; }
        }

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event type is required.")]
        public string EventType { get; set; } = string.Empty;

        public string RouteMapUrl { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid route.")]
        public int RouteId { get; set; }
    }
}