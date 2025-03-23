using DizimoParoquial.Data.Interface;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Dapper;
using MySql.Data.MySqlClient;
using System.Transactions;
using System.Collections.Generic;
using System.Text;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using System.Data.Common;

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
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro interno.", ex);
            }
        }

        public async Task<bool> RegisterUser(User user)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"INSERT INTO User(Name, Username, Password, Active, CreatedAt, UpdatedAt, Profile) 
                                    VALUES(@Name, @Username, @Password, @Active, @CreatedAt, @UpdatedAt, @Profile)";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                user.Name,
                                user.Username,
                                user.Password,
                                user.Active,
                                user.CreatedAt,
                                user.UpdatedAt,
                                user.Profile
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Usuário - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Usuário - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<List<UserDTO>> GetUsersWithouthFilters()
        {

            List<UserDTO> users = new List<UserDTO>();

            try
            {
                var query = "SELECT * FROM User WHERE Deleted = false and Username != 'admin';";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserDTO>(query);

                    users = result.ToList();

                    return users;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro interno.", ex);
            }
        }

        public async Task<List<UserDTO>> GetUsersWithFilters(bool? status, string? name)
        {

            List<UserDTO> users = new List<UserDTO>();

            try
            {
                StringBuilder query = new StringBuilder();
                
                query.Append("SELECT * FROM User WHERE Deleted = false and Username != 'admin' ");

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
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro interno.", ex);
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
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro interno.", ex);
            }
        }

        public async Task<UserDTO> GetUserById(int id)
        {

            UserDTO user = new UserDTO();

            try
            {
                var query = "SELECT * FROM User WHERE UserId = @UserId";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserDTO>(query,
                        new
                        {
                            UserId = id
                        }
                    );

                    user = result.FirstOrDefault();

                    return user;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Usuário - Erro interno.", ex);
            }
        }

        public async Task<bool> DeleteUser(int userId)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

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

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Deletar Usuário - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Deletar Usuário - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<bool> UpdateUser(User user)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"UPDATE User 
                                    SET Name = @Name, 
                                        Username = @Username, 
                                        Password = @Password, 
                                        Active = @Active, 
                                        UpdatedAt = @UpdatedAt 
                                    WHERE UserId = @UserId";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                user.Name,
                                user.Username,
                                user.Password,
                                user.Active,
                                user.UpdatedAt,
                                user.UserId
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Usuário - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Usuário - Erro interno.", ex);
                    }
                }
            }
        }

    }
}
