using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using System.Data.Common;

namespace DizimoParoquial.Services
{
    public class TitheService
    {

        private readonly ITitheRepository _titheRepository;
        private readonly IncomeService _incomeService;

        public TitheService(ITitheRepository titheRepository, IncomeService incomeService)
        {
            _titheRepository = titheRepository;
            _incomeService = incomeService;
        }

        public async Task<bool> SaveTithe(LauchingTithe lauchingTithe)
        {
            bool titheWasSaved = false;

            try
            {

                Income income = new Income
                {
                    AgentCode = lauchingTithe.AgentCode,
                    PaymentType = lauchingTithe.PaymentType,
                    Value = lauchingTithe.Value,
                    TithePayerId = lauchingTithe.TithePayerId,
                    UserId = null,
                    RegistrationDate = DateTime.Now
                };

                int incomeId = await _incomeService.SaveIncome(income);

                if (incomeId == 0)
                    throw new ValidationException("Salvar Dizimo - Erro ao salvar a entrada.");

                List<Tithe> tithesPaid = new List<Tithe>();

                int tithesCount = lauchingTithe.PaymentDates.Length;
                decimal tithesValueAmount = lauchingTithe.Value / tithesCount;

                for (int i = 0; i < tithesCount; i++)
                {
                    Tithe tithe = new Tithe
                    {
                        AgentCode = lauchingTithe.AgentCode,
                        PaymentType = lauchingTithe.PaymentType,
                        Value = tithesValueAmount,
                        TithePayerId = lauchingTithe.TithePayerId,
                        UserId = null,
                        RegistrationDate = DateTime.Now,
                        IncomeId = incomeId,
                        PaymentMonth = lauchingTithe.PaymentDates[i]
                    };

                    tithesPaid.Add(tithe);

                }

                titheWasSaved = await SaveTithesRepository(tithesPaid);

                return titheWasSaved;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Salvar Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Salvar Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Salvar Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Salvar Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Salvar Dizimo - Dados vazios.");
            }
        }

        public async Task<bool> UpdateTithe(Tithe tithe)
        {
            bool titheWasUpdated = false;

            try
            {

                titheWasUpdated = await UpdateTitheRepository(tithe);

                return titheWasUpdated;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar Dizimo - Dados vazios.");
            }
        }

        public async Task<List<TithePayerLaunchDTO>> GetTithesWithFilters(string? name, int tithePayerCode, string? document)
        {
            List<TithePayerLaunchDTO> tithes = new List<TithePayerLaunchDTO>();

            try
            {

                tithes = await GetTithesWithFiltersRepository(name, tithePayerCode, document);

                return tithes;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos - Dados vazios.");
            }
        }

        public async Task<List<TitheDTO>> GetTithesByTithePayerId(int tithePayerId)
        {
            List<TitheDTO> tithes = new List<TitheDTO>();

            try
            {

                tithes = await GetTithesByTithePayerIdRepository(tithePayerId);

                return tithes;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos - Dados vazios.");
            }
        }

        public async Task<List<TitheDTO>> GetReportTithesMonth(string paymentType, string name, DateTime startPaymentDate, DateTime endPaymentDate)
        {
            List<TitheDTO> tithes = new List<TitheDTO>();

            try
            {

                tithes = await GetReportTithesMonthRepository(paymentType, name, startPaymentDate, endPaymentDate);

                return tithes;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos - Dados vazios.");
            }
        }

        public async Task<TitheDTO> GetTitheById(int id)
        {
            TitheDTO tithe = new TitheDTO();

            try
            {

                tithe = await GetTitheByTitheIdRepository(id);

                return tithe;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos Detalhes - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos Detalhes - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos Detalhes - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos Detalhes - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos Detalhes - Dados vazios.");
            }
        }

        public async Task<bool> DeleteTithe(int titheId)
        {
            bool userWasDeleted = false;

            try
            {

                TitheDTO titheDelete = await GetTitheById(titheId);

                //userWasDeleted = await DeleteTitheRepository(titheId);

                return userWasDeleted;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Excluir Lançamento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Excluir Lançamento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Excluir Lançamento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Excluir Lançamento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Excluir Lançamento - Dados vazios.");
            }
        }

        #region Repositories Methods

        private async Task<bool> SaveTithesRepository(List<Tithe> tithes)
        {
            bool titheWasSaved = false;

            try
            {
                titheWasSaved = await _titheRepository.SaveTithes(tithes);

                return titheWasSaved;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Salvar Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Salvar Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Salvar Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Salvar Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Salvar Dizimo - Dados vazios.");
            }
        }

        private async Task<bool> UpdateTitheRepository(Tithe tithe)
        {
            bool titheWasUpdated = false;

            try
            {
                titheWasUpdated = await _titheRepository.UpdateTithe(tithe);

                return titheWasUpdated;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar Dizimo - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar Dizimo - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar Dizimo - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar Dizimo - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar Dizimo - Dados vazios.");
            }
        }

        private async Task<List<TithePayerLaunchDTO>> GetTithesWithFiltersRepository(string? name, int tithePayerCode, string? document)
        {
            List<TithePayerLaunchDTO> tithes = new List<TithePayerLaunchDTO>();

            try
            {
                tithes = await _titheRepository.GetTithesWithFilters(name, tithePayerCode, document);

                return tithes;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos - Dados vazios.");
            }
        }

        private async Task<List<TitheDTO>> GetTithesByTithePayerIdRepository(int tithePayerId)
        {
            List<TitheDTO> tithes = new List<TitheDTO>();

            try
            {
                tithes = await _titheRepository.GetTithesByTithePayerId(tithePayerId);

                return tithes;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos - Dados vazios.");
            }
        }

        private async Task<List<TitheDTO>> GetReportTithesMonthRepository(string paymentType, string name, DateTime startPaymentDate, DateTime endPaymentDate)
        {
            List<TitheDTO> tithes = new List<TitheDTO>();

            try
            {
                tithes = await _titheRepository.GetReportTithesMonth(paymentType, name, startPaymentDate, endPaymentDate);

                return tithes;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos - Dados vazios.");
            }
        }

        private async Task<TitheDTO> GetTitheByTitheIdRepository(int id)
        {
            TitheDTO tithe = new TitheDTO();

            try
            {
                tithe = await _titheRepository.GetTitheByTitheId(id);

                return tithe;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimos Detalhes - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimos Detalhes - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimos Detalhes - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimos Detalhes - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimos Detalhes - Dados vazios.");
            }
        }

        #endregion

    }
}
