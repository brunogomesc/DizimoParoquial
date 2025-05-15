namespace DizimoParoquial.Models
{
    public class ReportPaying
    {
        public int TithePayerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public Status StatusPaying { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? LastContribuition { get; set; }

        public int AmountContribuition { get; set; }
    }

    public enum Status
    {
        Adimplente,
        Inadimplente
    }
}
