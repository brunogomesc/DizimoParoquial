using DizimoParoquial.Data.Interface;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Dapper;
using MySql.Data.MySqlClient;
using System.Transactions;
using System.Collections.Generic;
using System.Text;
using DizimoParoquial.DTOs;

namespace DizimoParoquial.Data.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly ConfigurationService _configurationService;

        public UserRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<UserDTO> GetUserByUsernameAndPassword(string username, string password)
        {

            UserDTO user = new UserDTO();

            try
            {
                var query = "SELECT * FROM User WHERE Username = @Username AND Password = @Password and Active = @Active and Deleted = @Deleted;";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserDTO>(query,
                        new { 
                            Username = username, 
                            Password = password,
                            Active = true,
                            Deleted = false
                        }
                    );

                    user = result.FirstOrDefault();

                    return user;
                }
            }
            catch (Exception)
            {
                return user;
            }
        }

        public async Task<bool> RegisterUser(User user)
        {

            bool userWasCreated = false;

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {
                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"INSERT INTO User(Name, Username, Password, Active, CreatedAt, UpdatedAt) 
                                    VALUES(@Name, @Username, @Password, @Active, @CreatedAt, @UpdatedAt)";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                user.Name,
                                user.Username,
                                user.Password,
                                user.Active,
                                user.CreatedAt,
                                user.UpdatedAt
                            }
                        );

                        transaction.Commit();

                        return result > 0;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return userWasCreated;
                    }

                }
            }
        }

        public async Task<List<UserDTO>> GetUsersWithouthFilters()
        {

            List<UserDTO> users = new List<UserDTO>();

            try
            {
                var query = "SELECT * FROM User;";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserDTO>(query);

                    users = result.ToList();

                    return users;
                }
            }
            catch (Exception)
            {
                return users;
            }
        }

        public async Task<List<UserDTO>> GetUsersWithFilters(bool? status, string? name)
        {

            List<UserDTO> users = new List<UserDTO>();

            try
            {
                StringBuilder query = new StringBuilder();
                
                query.Append("SELECT * FROM User WHERE Deleted = false ");

                if (status != null)
                    query.Append($" AND Active = {status}");

                if(name != null)
                    query.Append($" AND Name LIKE '%{name}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserDTO>(query.ToString());

                    users = result.ToList();

                    return users;
                }
            }
            catch (Exception)
            {
                return users;
            }
        }

        public async Task<UserDTO> GetUserByUsername(string username)
        {

            UserDTO user = new UserDTO();

            try
            {
                var query = "SELECT * FROM User WHERE Username = @Username";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserDTO>(query,
                        new
                        {
                            Username = username
                        }
                    );

                    user = result.FirstOrDefault();

                    return user;
                }
            }
            catch (Exception)
            {
                return user;
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {

            bool userWasCreated = false;

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {
                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"UPDATE User SET Deleted = @Deleted, UpdatedAt = @UpdatedAt WHERE UserId = @UserId";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                Deleted = true,
                                UpdatedAt = DateTime.Now,
                                UserId = userId
                            }
                        );

                        transaction.Commit();

                        return result > 0;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return userWasCreated;
                    }

                }
            }
        }

    }
}
