namespace Site_3.Models
{
    public class ExportJob
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = "pending"; // pending, completed
        public string ExportData { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
