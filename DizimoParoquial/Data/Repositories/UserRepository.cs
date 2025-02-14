using DizimoParoquial.Data.Interface;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Dapper;
using MySql.Data.MySqlClient;

namespace DizimoParoquial.Data.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly ConfigurationService _configurationService;

        public UserRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {

            User user = new User();

            try
            {
                var query = $"SELECT * FROM User WHERE Username = '{username}' and Password = '{password}';";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    user = (User)await connection.QueryAsync<User>(query);

                    return user;
                }
            }
            catch (Exception ex)
            {
                return user;
            }
        }

    }
}
