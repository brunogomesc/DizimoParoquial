using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using System.Data.Common;

namespace DizimoParoquial.Services
{
    public class IncomeService
    {

        private readonly IIncomeRepository _incomeRepository;

        public IncomeService(IIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<int> SaveIncome(Income income)
        {
            try
            {

                int incomeId = await SaveIncomeRepository(income);

                return incomeId;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Salvar Entrada - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Salvar Entrada - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Salvar Entrada - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Salvar Entrada - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Salvar Entrada - Dados vazios.");
            }
        }

        #region Repositories Methods

        private async Task<int> SaveIncomeRepository(Income income)
        {
            int incomeId = 0;

            try
            {
                incomeId = await _incomeRepository.SaveIncome(income);

                return incomeId;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Salvar Entrada - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Salvar Entrada - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Salvar Entrada - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Salvar Entrada - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Salvar Entrada - Dados vazios.");
            }
        }

        #endregion

    }
}
