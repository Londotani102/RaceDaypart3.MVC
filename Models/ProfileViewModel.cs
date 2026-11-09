using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class ProfileViewModel
    {
        public int ParticipantId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = "Participant";
    }
}