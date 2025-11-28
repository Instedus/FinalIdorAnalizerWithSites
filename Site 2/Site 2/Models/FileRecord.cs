namespace Site_2.Models
{
    public class FileRecord
    {
        public string Id { get; set; } = string.Empty; // UUID в виде строки
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}