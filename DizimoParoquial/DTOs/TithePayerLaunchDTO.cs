namespace DizimoParoquial.DTOs
{
    public class TithePayerLaunchDTO
    {

        public int TithePayerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Document { get; set; } = string.Empty;

        public DateTime DateBirth { get; set; }

    }
}
