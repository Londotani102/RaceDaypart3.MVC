using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CategoryCreateViewModel
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Range(1, 100)]
        public int MinAge { get; set; }

        [Range(1, 100)]
        public int MaxAge { get; set; }

        [Range(0.1, 1000)]
        public double Distance { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an event.")]
        public int EventId { get; set; }
    }
}