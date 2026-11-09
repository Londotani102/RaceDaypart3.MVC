namespace RaceDay.MVC.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? EventType { get; set; }
        public string? RouteMapUrl { get; set; }
        public int RouteId { get; set; }
    }
}