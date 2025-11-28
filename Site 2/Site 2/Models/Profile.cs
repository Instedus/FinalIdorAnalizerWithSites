namespace Site_2.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Ссылка на владельца профиля
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
