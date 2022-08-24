using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.Dtos.Project
{
    public class CreateProjectDto
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; } = string.Empty;
        [Required]
        public bool IsPrivate { get; set; } = true;
        public IFormFile? File { get; set; }
    }
}
