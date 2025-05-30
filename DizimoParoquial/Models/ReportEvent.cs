namespace DizimoParoquial.Models
{
    public class ReportEvent
    {

        public int EventId { get; set; }

        public string Process { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string NameAgent { get; set; } = string.Empty;

        public string NameUser { get; set; } = string.Empty;

    }
}
