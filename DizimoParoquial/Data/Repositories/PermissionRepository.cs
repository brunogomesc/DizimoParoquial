using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Data.Common;

namespace DizimoParoquial.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {

        private readonly ConfigurationService _configurationService;

        public PermissionRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<List<UserPermissionDTO>> GetUserPermissions(int user)
        {

            List<UserPermissionDTO> userPermissions = new List<UserPermissionDTO>();

            try
            {
                var query = @"SELECT U.UserId, U.Username, UP.PermissionId, P.Name, P.Description 
                            FROM UserPermission UP
                            INNER JOIN User U
                            ON UP.UserId   = U.UserId 
                            INNER JOIN Permission P
                            ON UP.PermissionId = P.PermissionId
                            WHERE UP.UserId = @UserId";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<UserPermissionDTO>(query,
                        new
                        {
                            UserId = user
                        }
                    );

                    userPermissions = result.ToList();

                    return userPermissions;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Permissões - Erro interno.", ex);
            }
        }

        public async Task<List<Permission>> GetAllPermissions()
        {

            List<Permission> permissions = new List<Permission>();

            try
            {
                var query = @"SELECT * FROM Permission;";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<Permission>(query);

                    permissions = result.ToList();

                    return permissions;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Todas Permissões - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Todas Permissões - Erro interno.", ex);
            }
        }

        public async Task<bool> RegisterPermissions(int userId, List<int> selectedPermissionsInsertion)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {

                        int result = 0;

                        foreach (var item in selectedPermissionsInsertion)
                        {
                            var query = @"INSERT INTO UserPermission VALUES(@UserId, @PermissionId);";

                            result = await connection.ExecuteAsync(query,
                                new
                                {
                                    UserId = userId,
                                    PermissionId = item
                                }
                            );
  
                        }

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return result > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Permissões - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Permissões - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<bool> DeleteAllPermissionsByUser(int userId)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {

                        var query = @"DELETE FROM UserPermission WHERE UserId = @UserId;";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
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
                        throw new RepositoryException("Excluir Permissões - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Excluir Permissões - Erro interno.", ex);
                    }
                }
            }
        }

    }
}
