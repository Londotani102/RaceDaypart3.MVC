
namespace RaceDay.MVC.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int MinAge { get; set; }

        public int MaxAge { get; set; }

        public double Distance { get; set; }

        public int EventId { get; set; }
    }
}
