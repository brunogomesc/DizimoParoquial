namespace DizimoParoquial.Models
{
    public class ReportBirthday
    {

        public int TithePayerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public Status StatusPaying { get; set; }

        public string? Document { get; set; }

        public DateTime DateBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

    }
}
