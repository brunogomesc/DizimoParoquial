namespace DizimoParoquial.DTOs
{
    public class UserPermissionDTO
    {

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public int PermissionId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

    }
}
