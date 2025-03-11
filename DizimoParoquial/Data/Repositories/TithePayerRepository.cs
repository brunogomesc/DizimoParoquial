using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;

namespace DizimoParoquial.Data.Repositories
{
    public class TithePayerRepository : ITithePayerRepository
    {

        private readonly ConfigurationService _configurationService;

        public TithePayerRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<int> RegisterTithePayer(TithePayer tithePayer)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"INSERT INTO TithePayer(Name, Document, DateBirth, Email, PhoneNumber, Address, Number, ZipCode, Neighborhood, Complement, CreatedAt, UpdatedAt, TermFile, UserId) 
                                    VALUES(@Name, @Document, @DateBirth, @Email, @PhoneNumber, @Address, @Number, @ZipCode, @Neighborhood, @Complement, @CreatedAt, @UpdatedAt, @TermFile, @UserId);

                                    SELECT LAST_INSERT_ID();";

                        int idTithePayer = await connection.ExecuteScalarAsync<int>(query,
                            new
                            {
                                tithePayer.Name,
                                tithePayer.Document,
                                tithePayer.DateBirth,
                                tithePayer.Email,
                                tithePayer.PhoneNumber,
                                tithePayer.Address,
                                tithePayer.Number,
                                tithePayer.ZipCode,
                                tithePayer.Neighborhood,
                                tithePayer.Complement,
                                tithePayer.CreatedAt,
                                tithePayer.UpdatedAt,
                                tithePayer.TermFile,
                                tithePayer.UserId
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return idTithePayer;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Dizimista - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Criar Dizimista - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<TithePayer> GetTithePayerById(int id)
        {

            TithePayer tithePayer = new TithePayer();

            try
            {
                var query = "SELECT * FROM TithePayer WHERE TithePayerId = @TithePayerId";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TithePayer>(query,
                        new
                        {
                            TithePayerId = id
                        }
                    );

                    tithePayer = result.FirstOrDefault();

                    return tithePayer;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Dizimista - Erro interno.", ex);
            }
        }

        public async Task<List<TithePayer>> GetTithePayersWithouthFilters()
        {

            List<TithePayer> tithePayers = new List<TithePayer>();

            try
            {
                var query = "SELECT * FROM TithePayer;";

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TithePayer>(query);

                    tithePayers = result.ToList();

                    return tithePayers;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Dizimista - Erro interno.", ex);
            }
        }

        public async Task<List<TithePayer>> GetTithePayersWithFilters(string? document, string? name)
        {

            List<TithePayer> tithePayers = new List<TithePayer>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM TithePayer WHERE 1 = 1 ");

                if (document != null)
                    query.Append($" AND Document = '{document}'");

                if (name != null)
                    query.Append($" AND Name LIKE '%{name}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TithePayer>(query.ToString());

                    tithePayers = result.ToList();

                    return tithePayers;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Dizimista - Erro interno.", ex);
            }
        }

        public async Task<TithePayerLaunchDTO> GetTithePayersLauchingWithFilters(string? name, int code)
        {

            TithePayerLaunchDTO tithePayer = new TithePayerLaunchDTO();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM TithePayer WHERE 1 = 1 ");

                if (code != 0)
                    query.Append($" AND TithePayerId = {code}");

                if (name != null)
                    query.Append($" AND Name LIKE '%{name}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TithePayerLaunchDTO>(query.ToString());

                    tithePayer = result.FirstOrDefault();

                    return tithePayer;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Dizimista Lançamento - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Dizimista Lançamento - Erro interno.", ex);
            }
        }

        public async Task<bool> UpdateTithePayer(TithePayer tithePayer)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"UPDATE TithePayer 
                                    SET Name = @Name, 
                                        Document = @Document,
                                        DateBirth = @DateBirth,
                                        Email = @Email,
                                        PhoneNumber = @PhoneNumber,
                                        Address = @Address,
                                        Number = @Number,
                                        ZipCode = @ZipCode,
                                        Neighborhood = @Neighborhood,
                                        Complement = @Complement,
                                        UpdatedAt = @UpdatedAt
                                        TermFile = @TermFile
                                    WHERE TithePayerId = @TithePayerId";

                        var result = await connection.ExecuteAsync(query,
                            new
                            {
                                tithePayer.Name,
                                tithePayer.Document,
                                tithePayer.DateBirth,
                                tithePayer.Email,
                                tithePayer.PhoneNumber,
                                tithePayer.Address,
                                tithePayer.Number,
                                tithePayer.ZipCode,
                                tithePayer.Neighborhood,
                                tithePayer.Complement,
                                tithePayer.UpdatedAt,
                                tithePayer.TermFile,
                                tithePayer.TithePayerId
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
                        throw new RepositoryException("Atualizar Dizimista - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Dizimista - Erro interno.", ex);
                    }
                }
            }
        }

    }
}
