using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.Dtos.Team
{
    public class CreateTeamDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Username { get; set; }
    }
}
