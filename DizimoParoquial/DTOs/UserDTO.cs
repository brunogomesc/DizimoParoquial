namespace DizimoParoquial.DTOs
{
    public class UserDTO
    {

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool Active { get; set; }

        public string Profile { get; set; } = string.Empty;

        public bool SuperUser { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
