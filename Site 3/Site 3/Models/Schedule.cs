namespace Site_3.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CronExpression { get; set; } = "0 0 * * *";
        public DateTime LastRun { get; set; }
        public string JobType { get; set; } = "export";
    }
}
