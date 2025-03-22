using Dapper;
using DizimoParoquial.Data.Interface;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using MySql.Data.MySqlClient;
using System.Data.Common;

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

    }
}
