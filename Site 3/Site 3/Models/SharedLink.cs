namespace Site_3.Models
{
    public class SharedLink
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int UserId { get; set; }
        public string TargetId { get; set; } = string.Empty;
        public string TargetType { get; set; } = "document";
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}
