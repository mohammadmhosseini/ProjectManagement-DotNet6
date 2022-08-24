using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectManagement.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public bool? IsPrivate { get; set; } = true;
        [JsonIgnore]
        public User User { get; set; }
        public int UserId { get; set; }
        public List<Team> Teams { get; set; }
    }
}
