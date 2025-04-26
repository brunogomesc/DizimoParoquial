namespace DizimoParoquial.DTOs
{
    public class TithePayerDTO
    {

        public int TithePayerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Document { get; set; }

        public DateTime DateBirth { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string Neighborhood { get; set; } = string.Empty;

        public string Complement { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public IFormFile? TermFile { get; set; }

    }
}
