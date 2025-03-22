namespace DizimoParoquial.Models
{
    public class ReportTithePayer
    {

        public int IncomeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Value { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentType { get; set; } = string.Empty;

    }
}
