namespace DizimoParoquial.DTOs
{
    public class TitheDTO
    {

        public int TitheId { get; set; }

        public int TithePayerId { get; set; }

        public string NameTithePayer { get; set; } = string.Empty;

        public string NameAgent { get; set; } = string.Empty;

        public string PaymentType { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public DateTime PaymentMonth { get; set; }

        public DateTime RegistrationDate { get; set; }

    }
}
