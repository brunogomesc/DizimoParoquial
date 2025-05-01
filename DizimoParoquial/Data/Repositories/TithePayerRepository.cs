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

                        var queryDuplicate = "SELECT COUNT(*) FROM TithePayer WHERE Document = @Document";

                        int? tithePayerExistent = await connection.ExecuteScalarAsync<int?>(queryDuplicate,
                            new
                            {
                                tithePayer.Document
                            }
                        );

                        int idTithePayer = 0;

                        if (tithePayerExistent == null || tithePayerExistent == 0)
                        {
                            throw new RepositoryException("Documento da pessoa já existe no sistema!");
                        }
                        else
                        {
                            var query = @"INSERT INTO TithePayer(Name, Document, DateBirth, Email, PhoneNumber, Address, Number, ZipCode, Neighborhood, Complement, CreatedAt, UpdatedAt, TermFile, UserId, Extension) 
                                    VALUES(@Name, @Document, @DateBirth, @Email, @PhoneNumber, @Address, @Number, @ZipCode, @Neighborhood, @Complement, @CreatedAt, @UpdatedAt, @TermFile, @UserId, @Extension);

                                    SELECT LAST_INSERT_ID();";

                            idTithePayer = await connection.ExecuteScalarAsync<int>(query,
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
                                    tithePayer.UserId,
                                    tithePayer.Extension
                                }
                            );
                        }

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

        public async Task<TithePayerLaunchDTO> GetTithePayersLauchingWithFilters(int code)
        {

            TithePayerLaunchDTO tithePayer = new TithePayerLaunchDTO();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM TithePayer WHERE 1 = 1 ");

                if (code != 0)
                    query.Append($" AND TithePayerId = {code}");

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

        public async Task<List<TithePayerLaunchDTO>> GetTithePayersLauchingWithFilters(string? name, int code)
        {

            List<TithePayerLaunchDTO> tithePayer = new List<TithePayerLaunchDTO>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM TithePayer WHERE 1 = 1 ");

                if (code != 0)
                    query.Append($" AND TithePayerId = {code}");

                if (name != null)
                    query.Append($" AND Name LIKE '%{name}%'");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TithePayerLaunchDTO>(query.ToString());

                    tithePayer = result.ToList();

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
                                        UpdatedAt = @UpdatedAt,
                                        TermFile = @TermFile,
                                        Extension = @Extension
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
                                tithePayer.Extension,
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

        public async Task<List<ReportTithePayer>> GetReportTithePayers(string? paymentType, string? name, DateTime startPaymentDate, DateTime endPaymentDate)
        {

            List<ReportTithePayer> report = new List<ReportTithePayer>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT I.IncomeId, TP.Name, I.Value, I.PaymentType, I.RegistrationDate as PaymentDate ");
                query.Append("FROM Income I ");
                query.Append("INNER JOIN TithePayer TP ");
                query.Append("ON I.TithePayerId = TP.TithePayerId ");
                query.Append($"WHERE RegistrationDate BETWEEN '{startPaymentDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{endPaymentDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}' ");

                if (paymentType != null)
                    query.Append($"AND PaymentType = '{paymentType}' ");

                if (name != null)
                    query.Append($"AND Name LIKE '%{name}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<ReportTithePayer>(query.ToString());

                    report = result.ToList();

                    return report;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Relatório Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Relatório Dizimista - Erro interno.", ex);
            }
        }

        public async Task<List<ReportBirthday>> GetReportTithePayersBirthdays(string? name, DateTime startBirthdayDate, DateTime endBirthdayDate)
        {

            List<ReportBirthday> report = new List<ReportBirthday>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT TP.TithePayerId, TP.Name, TP.Document, TP.DateBirth, TP.PhoneNumber, TP.Email ");
                query.Append("FROM TithePayer TP ");
                query.Append($"WHERE MONTH(TP.DateBirth) BETWEEN '{startBirthdayDate.Month}' AND '{endBirthdayDate.Month}' ");
                query.Append($"AND DAY(TP.DateBirth) BETWEEN '{startBirthdayDate.Day}' AND '{endBirthdayDate.Day}' ");

                if (name != null)
                    query.Append($"AND TP.Name LIKE '%{name}%' ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<ReportBirthday>(query.ToString());

                    report = result.ToList();

                    return report;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Relatório Aniversariante - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Relatório Aniversariante - Erro interno.", ex);
            }
        }

    }
}
