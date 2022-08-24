namespace ProjectManagement.Models.Dtos.Project
{
    public class EditProjectDto
    {
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool? IsPrivate { get; set; } = true;
        public IFormFile? File { get; set; }
    }
}
