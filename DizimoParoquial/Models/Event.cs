namespace DizimoParoquial.Models
{
    public class Event
    {

        public int EventId { get; set; }

        public DateTime EventDate { get; set; }

        public string Process { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public int? UserId { get; set; }

        public int? AgentId { get; set; }

    }
}
