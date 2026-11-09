
namespace RaceDay.MVC.Models
{
    public class EnrolmentResultViewModel
    {
        public int EventEnrolmentId { get; set; }

        public int EventId { get; set; }

        public int CategoryId { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
