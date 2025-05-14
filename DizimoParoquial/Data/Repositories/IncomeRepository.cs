using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls.Crypto;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace DizimoParoquial.Data.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {

        private readonly ConfigurationService _configurationService;

        public IncomeRepository(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<int> SaveIncome(Income income)
        {

            using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
            {

                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    try
                    {
                        var query = @"INSERT INTO Income(AgentCode, PaymentType, Value, TithePayerId, UserId, RegistrationDate) 
                                    VALUES(@AgentCode, @PaymentType, @Value, @TithePayerId, @UserId, @RegistrationDate);

                                    SELECT LAST_INSERT_ID();"
                        ;

                        int incomeId = await connection.ExecuteScalarAsync<int>(query,
                        new
                        {
                            income.AgentCode,
                            income.PaymentType,
                            income.Value,
                            income.TithePayerId,
                            income.UserId,
                            income.RegistrationDate
                        }
                        );

                        transaction.Commit();

                        await connection.DisposeAsync();

                        return incomeId;
                    }
                    catch (DbException ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Salvar Entrada - Erro ao acessar o banco de dados.", ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        await connection.DisposeAsync();
                        throw new RepositoryException("Salvar Entrada - Erro interno.", ex);
                    }
                }
            }
        }

        public async Task<List<ReportSum>> GetReportSum(string? paymentType, DateTime startPaymentDate, DateTime endPaymentDate)
        {

            List<ReportSum> report = new List<ReportSum>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT I.PaymentType, ");
                query.Append("SUM(Value) AS TotalValue, ");
                query.Append("COUNT(PaymentType) AS AmountPayments ");
                query.Append("FROM Income I ");
                query.Append($"WHERE I.RegistrationDate BETWEEN '{startPaymentDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{endPaymentDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}' ");
                query.Append("AND I.Value != 0 ");

                if (paymentType != null)
                    query.Append($"AND I.PaymentType = '{paymentType}' ");

                query.Append("GROUP BY I.PaymentType ");
                query.Append("UNION ");
                query.Append("SELECT 'Total' AS PaymentType, ");
                query.Append("SUM(Value) as TotalValue, ");
                query.Append("COUNT(PaymentType) AS AmountPayments ");
                query.Append("FROM Income I ");
                query.Append($"WHERE I.RegistrationDate BETWEEN '{startPaymentDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{endPaymentDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")}' ");
                query.Append("AND I.Value != 0; ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<ReportSum>(query.ToString());

                    report = result.ToList();

                    return report;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Relatório de Totais - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Relatório de Totais - Erro interno.", ex);
            }
        }

        public async Task<List<ReportSumAddress>> GetReportSumAddress()
        {

            List<ReportSumAddress> report = new List<ReportSumAddress>();

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT TP.Address, ");
                query.Append("TP.Neighborhood, ");
                query.Append("COUNT(TP.TithePayerId) as AmountAddress ");
                query.Append("FROM TithePayer TP ");
                query.Append("GROUP BY TP.Address, TP.Neighborhood ");
                query.Append("UNION ");
                query.Append("SELECT 'Total Dizimistas' AS Address, ");
                query.Append("' ' AS Neighborhood, ");
                query.Append("COUNT(TP.TithePayerId) as AmountAddress ");
                query.Append("FROM TithePayer TP; ");

                using (var connection = new MySqlConnection(_configurationService.GetConnectionString()))
                {
                    var result = await connection.QueryAsync<ReportSumAddress>(query.ToString());

                    report = result.ToList();

                    return report;
                }
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consulta Relatório de Ruas - Erro ao acessar o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Consulta Relatório de Ruas - Erro interno.", ex);
            }
        }

    }
}
