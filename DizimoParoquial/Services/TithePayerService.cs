using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using System.Data.Common;

namespace DizimoParoquial.Services
{
    public class TithePayerService
    {

        private readonly ITithePayerRepository _tithePayerRepository;

        public TithePayerService(ITithePayerRepository tithePayerRepository)
        {
            _tithePayerRepository = tithePayerRepository;
        }

        public async Task<int> RegisterTithePayer(TithePayerDTO tithePayer)
        {
            int tithePayerCode = 0;

            try
            {

                byte[]? imageBytes = null;

                if (tithePayer.TermFile != null)
                {
                    using var memoryStream = new MemoryStream();
                    await tithePayer.TermFile.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                TithePayer tithePayerInsert = new TithePayer
                {
                    Name = tithePayer.Name,
                    Document = tithePayer.Document.Replace(".","").Replace("-",""),
                    DateBirth = tithePayer.DateBirth,
                    Email = tithePayer.Email,
                    PhoneNumber = tithePayer.PhoneNumber,
                    Address = tithePayer.Address,
                    Number = tithePayer.Number,
                    ZipCode = tithePayer.ZipCode,
                    Neighborhood = tithePayer.Neighborhood,
                    Complement = tithePayer.Complement,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    TermFile = imageBytes
                };

                tithePayerCode = await RegisterTithePayerRepository(tithePayerInsert);

                return tithePayerCode;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar Dizimista - Dados vazios.");
            }
        }

        public async Task<TithePayer> GetTithePayerById(int id)
        {
            TithePayer tithePayer = new TithePayer();

            try
            {
                tithePayer = await GetTithePayerByIdRepository(id);

                return tithePayer;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Buscar Detalhes Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Buscar Detalhes Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Buscar Detalhes Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Buscar Detalhes Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Buscar Detalhes Dizimista - Dados vazios.");
            }
        }

        public async Task<List<TithePayer>> GetTithePayersWithouthFilters()
        {
            List<TithePayer> tithePayers = new List<TithePayer>();

            try
            {

                tithePayers = await GetTithePayersWithouthFiltersRepository();

                return tithePayers;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimistas - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimistas - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimistas - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimistas - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimistas - Dados vazios.");
            }
        }

        public async Task<List<TithePayer>> GetTithePayersWithFilters(string? document, string? name)
        {
            List<TithePayer> tithePayers = new List<TithePayer>();

            try
            {

                tithePayers = await GetTithePayersWithFiltersRepository(document, name);

                return tithePayers;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista - Dados vazios.");
            }
        }

        public async Task<bool> UpdateTithePayer(TithePayerDTO tithePayer)
        {
            bool tithePayerWasUpdated = false;

            try
            {

                TithePayer tithePayerExists = await GetTithePayerById(tithePayer.TithePayerId);

                if (tithePayerExists == null && tithePayerExists.TithePayerId == 0)
                    throw new ValidationException("Dizimista não existente");

                tithePayer.UpdatedAt = DateTime.Now;

                TithePayer tithePayerUpdate = new TithePayer
                {
                    TithePayerId = tithePayer.TithePayerId,
                    Name = tithePayer.Name,
                    Document = tithePayer.Document.Replace(".", "").Replace("-", ""),
                    DateBirth = tithePayer.DateBirth,
                    Email = tithePayer.Email,
                    PhoneNumber = tithePayer.PhoneNumber,
                    Address = tithePayer.Address,
                    Number = tithePayer.Number,
                    ZipCode = tithePayer.ZipCode,
                    Neighborhood = tithePayer.Neighborhood,
                    Complement = tithePayer.Complement,
                    CreatedAt = tithePayerExists.CreatedAt,
                    UpdatedAt = tithePayer.UpdatedAt,
                    TermFile = tithePayerExists.TermFile
                };

                tithePayerWasUpdated = await UpdateTithePayerRepository(tithePayerUpdate);

                return tithePayerWasUpdated;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar Dizimista - Dados vazios.");
            }
        }

        #region Repositories Methods

        private async Task<int> RegisterTithePayerRepository(TithePayer tithePayer)
        {
            int tithePayerCode = 0;

            try
            {
                tithePayerCode = await _tithePayerRepository.RegisterTithePayer(tithePayer);

                return tithePayerCode;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Criar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Criar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Criar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Criar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Criar Dizimista - Dados vazios.");
            }
        }

        private async Task<TithePayer> GetTithePayerByIdRepository(int id)
        {
            TithePayer tithePayer = new TithePayer();

            try
            {
                tithePayer = await _tithePayerRepository.GetTithePayerById(id);

                return tithePayer;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Buscar Detalhes Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Buscar Detalhes Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Buscar Detalhes Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Buscar Detalhes Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Buscar Detalhes Dizimista - Dados vazios.");
            }
        }

        private async Task<List<TithePayer>> GetTithePayersWithouthFiltersRepository()
        {
            List<TithePayer> tithePayers = new List<TithePayer>();

            try
            {
                tithePayers = await _tithePayerRepository.GetTithePayersWithouthFilters();

                return tithePayers;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista - Dados vazios.");
            }
        }

        private async Task<List<TithePayer>> GetTithePayersWithFiltersRepository(string? document, string? name)
        {
            List<TithePayer> tithePayers = new List<TithePayer>();

            try
            {
                tithePayers = await _tithePayerRepository.GetTithePayersWithFilters(document, name);

                return tithePayers;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista - Dados vazios.");
            }
        }

        private async Task<bool> UpdateTithePayerRepository(TithePayer tithePayer)
        {
            bool tithePayerWasUpdated = false;

            try
            {
                tithePayerWasUpdated = await _tithePayerRepository.UpdateTithePayer(tithePayer);

                return tithePayerWasUpdated;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Atualizar Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Atualizar Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Atualizar Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Atualizar Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Atualizar Dizimista - Dados vazios.");
            }
        }

        #endregion

    }
}
