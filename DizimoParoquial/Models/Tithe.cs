namespace DizimoParoquial.Models
{
    public class Tithe
    {

        public int TitheId { get; set; }

        public int TithePayerId { get; set; }

        public string AgentCode { get; set; } = string.Empty;

        public string PaymentType { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public int IncomeId { get; set; }

        public DateTime PaymentMonth { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int? UserId { get; set; }

    }
}
