using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class ResultCreateViewModel
    {
        [Required(ErrorMessage = "Enter the enrolment ID.")]
        [Range(1, int.MaxValue)]
        public int EventEnrollmentId { get; set; }

        [Required(ErrorMessage = "Enter the finishing time.")]
        public TimeSpan FinishTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Position must be at least 1.")]
        public int Position { get; set; }
    }
}