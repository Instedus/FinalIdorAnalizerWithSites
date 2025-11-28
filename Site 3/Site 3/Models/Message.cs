namespace Site_3.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = "draft"; // draft, sent
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
