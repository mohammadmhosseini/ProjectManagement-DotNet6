using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectManagement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^[a-z]+[a-z0-9\.\_]{3,}", ErrorMessage = "فرمت نام کاربری صحیح نمیباشد")]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public string? ProfileImage { get; set; } = string.Empty;
        public string? Token { get; set; } = string.Empty;
        public Project? Project { get; set; }
        public Team? Team { get; set; }
        public List<InviteRequest>? InviteRequests { get; set; }
    }
}