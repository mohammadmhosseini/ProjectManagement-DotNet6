namespace ProjectManagement.Models
{
    public class InviteRequest
    {
        public int Id { get; set; }
        public int teamId { get; set; }
        public string Caller { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}
