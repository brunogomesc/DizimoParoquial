namespace DizimoParoquial.Models
{
    public class Agent
    {

        public int AgentId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool Active { get; set; }

        public string AgentCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
