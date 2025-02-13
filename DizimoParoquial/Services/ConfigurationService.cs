namespace DizimoParoquial.Services
{

    public class ConfigurationService
    {

        private readonly string _connectionString;

        public ConfigurationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

    }
}
