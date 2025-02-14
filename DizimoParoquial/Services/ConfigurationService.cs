namespace DizimoParoquial.Services
{

    public class ConfigurationService
    {

        private readonly string _connectionString;

        private const string _KEY_ENCRYPTION = "@dizimoPAROQUIAL2025";

        public ConfigurationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        public string GetKeyEncryption()
        {
            return _KEY_ENCRYPTION;
        }

    }
}
