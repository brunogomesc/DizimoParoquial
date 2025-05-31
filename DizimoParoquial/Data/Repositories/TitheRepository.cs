using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;
using System.Xml.Linq;

namespace DizimoParoquial.Data.Repositories
{
    public class TitheRepository : ITitheRepository
    {

        private readonly ConfigurationService _configurationService;

        public TitheRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<bool> SaveTithes(List<Tithe> tithes)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {

                        var query = @"INSERT INTO Tithe(TithePayerId, AgentCode, PaymentType, Value, IncomeId, PaymentMonth, RegistrationDate, UserId) 
                                    VALUES(@TithePayerId, @AgentCode, @PaymentType, @Value, @IncomeId, @PaymentMonth, @RegistrationDate, @UserId);";

                        int rowsAffected = 0;

                        foreach (var tithe in tithes)
                        {

                            var registrationUTC = tithe.RegistrationDate.AddHours(-3);

                            rowsAffected = await connection.ExecuteAsync(query,
                                new
                                {
                                    tithe.TithePayerId,
                                    tithe.AgentCode,
                                    tithe.PaymentType,
                                    tithe.Value,
                                    tithe.IncomeId,
                                    tithe.PaymentMonth,
                                    RegistrationDate = registrationUTC,
                                    tithe.UserId
                                }
                            );
                        }

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return rowsAffected > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Salvar Dizimo - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Salvar Dizimo - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<bool> UpdateTithe(Tithe tithe, decimal oldValue)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"UPDATE Tithe SET AgentCode = @AgentCode, PaymentMonth = @PaymentMonth, 
                                    PaymentType = @PaymentType, TithePayerId = @TithePayerId, Value = @Value
                                    WHERE TitheId = @TitheId;";

                        int rowsAffected = 0;
                        
                        rowsAffected = await connection.ExecuteAsync(query,
                            new
                            {
                                tithe.TithePayerId,
                                tithe.AgentCode,
                                tithe.PaymentType,
                                tithe.Value,
                                tithe.PaymentMonth,
                                tithe.TitheId
                            }
                        );

                        StringBuilder queryIncome = new StringBuilder();

                        queryIncome.Append("CALL UpdateIncomeValue(@titheId, @newValue, @oldValue);");

                        int rowsAffectedIncome = 0;

                        rowsAffectedIncome = await connection.ExecuteAsync(queryIncome.ToString(),
                            new
                            {
                                tithe.TitheId,
                                newValue = tithe.Value,
                                oldValue
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return rowsAffected > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Dizimo - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Dizimo - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<List<TithePayerLaunchDTO>> GetTithesWithFilters(string? name, int tithePayerCode, string? document)
        {

            List<TithePayerLaunchDTO> tithes = new List<TithePayerLaunchDTO>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM TithePayer TP ");
                query.Append("WHERE 1 = 1 ");

                if (name != null)
                    query.Append($" AND TP.Name LIKE '%{name.TrimEnd().TrimStart()}%'");

                if (tithePayerCode != 0)
                    query.Append($" AND TP.TithePayerId = {tithePayerCode} ");

                if (document != null)
                    query.Append($" AND TP.Document = '{document}'");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TithePayerLaunchDTO>(query.ToString());

                    tithes = result.ToList();

                    return tithes;
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

        public async Task<List<TitheDTO>> GetTithesByTithePayerId(int tithePayerId)
        {

            List<TitheDTO> tithes = new List<TitheDTO>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT T.TitheId, ");
                query.Append("TP.TithePayerId, ");
                query.Append("TP.Name as NameTithePayer, ");
                query.Append("A.Name as NameAgent, ");
                query.Append("T.PaymentType, ");
                query.Append("T.Value, ");
                query.Append("T.PaymentMonth, ");
                query.Append("T.RegistrationDate ");
                query.Append("FROM TithePayer TP ");
                query.Append("LEFT JOIN Tithe T ");
                query.Append("ON T.TithePayerId = TP.TithePayerId ");
                query.Append("LEFT JOIN Agent A ");
                query.Append("ON T.AgentCode = A.AgentCode ");
                query.Append("WHERE 1 = 1 ");

                if (tithePayerId != 0)
                    query.Append($" AND T.TithePayerId = {tithePayerId}");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TitheDTO>(query.ToString());

                    tithes = result.ToList();

                    return tithes;
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

        public async Task<List<TitheDTO>> GetReportTithesMonth(string? paymentType, string? name, DateTime startPaymentDate, DateTime endPaymentDate)
        {

            List<TitheDTO> tithes = new List<TitheDTO>();

            try
            {
                StringBuilder query = new StringBuilder();

                string finalDate = $"{endPaymentDate.Year}-{endPaymentDate.Month}-{DateTime.DaysInMonth(endPaymentDate.Year, endPaymentDate.Month)} 23:59:59";

                query.Append("SELECT T.TitheId, ");
                query.Append("TP.TithePayerId, ");
                query.Append("TP.Name as NameTithePayer, ");
                query.Append("A.Name as NameAgent, ");
                query.Append("T.PaymentType, ");
                query.Append("T.Value, ");
                query.Append("T.PaymentMonth, "); 
                query.Append("T.RegistrationDate ");
                query.Append("FROM TithePayer TP ");
                query.Append("INNER JOIN Tithe T ");
                query.Append("ON T.TithePayerId = TP.TithePayerId ");
                query.Append("LEFT JOIN Agent A ");
                query.Append("ON T.AgentCode = A.AgentCode ");
                query.Append("WHERE T.TitheId IS NOT NULL ");
                query.Append($"AND PaymentMonth BETWEEN '{startPaymentDate.Year}-{startPaymentDate.Month}-01 00:00:00' AND '{finalDate}' ");

                if (paymentType != null)
                    query.Append($" AND T.PaymentType = '{paymentType}' ");

                if (name != null)
                    query.Append($" AND TP.Name = '{name}' W");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TitheDTO>(query.ToString());

                    tithes = result.ToList();

                    return tithes;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Dizimo - Erro interno.", ex);
            }
        }

        public async Task<TitheDTO> GetTitheByTitheId(int id)
        {

            TitheDTO tithe = new TitheDTO();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT T.TitheId, ");
                query.Append("TP.TithePayerId, ");
                query.Append("TP.Name as NameTithePayer, ");
                query.Append("A.Name as NameAgent, ");
                query.Append("T.PaymentType, ");
                query.Append("T.Value, ");
                query.Append("T.PaymentMonth, ");
                query.Append("T.RegistrationDate ");
                query.Append("FROM TithePayer TP ");
                query.Append("LEFT JOIN Tithe T ");
                query.Append("ON T.TithePayerId = TP.TithePayerId ");
                query.Append("LEFT JOIN Agent A ");
                query.Append("ON T.AgentCode = A.AgentCode ");
                query.Append("WHERE 1 = 1 ");

                if (id != 0)
                    query.Append($" AND T.TitheId = {id}");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<TitheDTO>(query.ToString());

                    tithe = result.FirstOrDefault();

                    return tithe;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Dizimista Detalhes - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Dizimista Detalhes - Erro interno.", ex);
            }
        }

        public async Task<bool> DeleteTithe(int titheId)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = $"CALL DeleteTithe(@titheId);";

                        int rowsAffected = 0;

                        rowsAffected = await connection.ExecuteAsync(query,
                            new
                            {
                                titheId,
                            }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return rowsAffected > 0;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Dizimo - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Atualizar Dizimo - Erro interno.", ex);
                    }
                }
            }
        }

    }
}
