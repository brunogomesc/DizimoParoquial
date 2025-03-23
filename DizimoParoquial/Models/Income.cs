namespace DizimoParoquial.Models
{
    public class Income
    {

        public int IncomeId { get; set; }

        public string AgentCode { get; set; } = string.Empty;

        public string PaymentType { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public int TithePayerId { get; set; }

        public int? UserId { get; set; }

        public DateTime RegistrationDate { get; set; }

    }
}
