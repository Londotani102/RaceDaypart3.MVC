
using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant";
    }
}
