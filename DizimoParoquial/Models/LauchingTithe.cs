namespace DizimoParoquial.Models
{
    public class LauchingTithe
    {

        public decimal Value { get; set; }

        public string AgentCode { get; set; } = string.Empty;

        public required DateTime[] PaymentDates { get; set; }

        public string PaymentType { get; set; } = string.Empty;

        public int TithePayerId { get; set; }

    }
}
