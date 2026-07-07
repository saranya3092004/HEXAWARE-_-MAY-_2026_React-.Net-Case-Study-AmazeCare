namespace AmazeCare.Server.Models
{
        public class Admin
        {
            public int AdminId { get; set; }
            public int UserId { get; set; }                          
            public string Name { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Navigation property
            public User User { get; set; } = null!; //one-one relationship with User
    }
    
}
