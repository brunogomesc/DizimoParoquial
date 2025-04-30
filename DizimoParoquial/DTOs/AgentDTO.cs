namespace DizimoParoquial.DTOs
{
    public class AgentDTO
    {

        public int AgentId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string AgentCode { get; set; } = string.Empty;

        public bool Active { get; set; }

    }
}
