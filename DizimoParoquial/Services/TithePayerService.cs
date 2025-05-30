using DizimoParoquial.Data.Interface;
using DizimoParoquial.Data.Repositories;
using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using System.Data.Common;
using System.Xml.Linq;

namespace DizimoParoquial.Services
{
    public class TithePayerService
    {

        private readonly ITithePayerRepository _tithePayerRepository;
        private readonly TitheService _titheService;

        public TithePayerService(ITithePayerRepository tithePayerRepository, TitheService titheService)
        {
            _tithePayerRepository = tithePayerRepository;
            _titheService = titheService;
        }

        public async Task<int> RegisterTithePayer(TithePayerDTO tithePayer, int userId)
        {
            int tithePayerCode = 0;

            try
            {

                if(tithePayer.Document != null)
                {
                    List<TithePayer> tithePayerExisting = await GetTithePayersWithFilters(tithePayer.Document, null);

                    if (tithePayerExisting != null && tithePayerExisting.Count > 0)
                        throw new ValidationException("Dizimista já cadastrado");
                }

                byte[]? imageBytes = null;
                string? extension = null;

                if (tithePayer.TermFile != null && tithePayer.TermFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await tithePayer.TermFile.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();

                    extension = Path.GetExtension(tithePayer.TermFile.FileName);

                }

                TithePayer tithePayerInsert = new TithePayer
                {
                    Name = tithePayer.Name,
                    Document = tithePayer.Document != null ? tithePayer.Document.Replace(".", "").Replace("-", "") : null,
                    DateBirth = tithePayer.DateBirth,
                    Email = tithePayer.Email,
                    PhoneNumber = tithePayer.PhoneNumber,
                    Address = tithePayer.Address,
                    Number = tithePayer.Number,
                    ZipCode = tithePayer.ZipCode != null ? tithePayer.ZipCode.Replace("-", "") : null,
                    Neighborhood = tithePayer.Neighborhood,
                    Complement = tithePayer.Complement,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    TermFile = imageBytes,
                    Extension = extension,
                    UserId = userId
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

        public async Task<List<ReportTithePayer>> GetReportTithePayers(string? paymentType, string? name, DateTime startPaymentDate, DateTime endPaymentDate)
        {
            List<ReportTithePayer> report = new List<ReportTithePayer>();

            try
            {

                report = await GetReportTithePayersRepository(paymentType, name, startPaymentDate, endPaymentDate);

                return report;

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

        public async Task<List<ReportBirthday>> GetReportTithePayersBirthdays(string? name, DateTime startBirthdayDate, DateTime endBirthdayDate)
        {
            List<ReportBirthday> report = new List<ReportBirthday>();

            try
            {

                List<ReportBirthday> tithePayers = await GetReportTithePayersBirthdaysRepository(name, startBirthdayDate, endBirthdayDate);

                foreach (var person in tithePayers)
                {

                    List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(person.TithePayerId);

                    if (tithes.Count == 0)
                    {
                        report.Add(new ReportBirthday
                        {
                            TithePayerId = person.TithePayerId,
                            Name = person.Name,
                            StatusPaying = Status.NaoContribuinte,
                            Document = person.Document,
                            DateBirth = person.DateBirth,
                            PhoneNumber = person.PhoneNumber,
                            Email = person.Email
                        });
                    }
                    else
                    {

                        int amountMonths = CalculatePaidMonthsLastSemester(tithes);

                        report.Add(new ReportBirthday
                        {
                            TithePayerId = person.TithePayerId,
                            Name = person.Name,
                            StatusPaying = amountMonths == 0 ? Status.Inadimplente : Status.Adimplente,
                            Document = person.Document,
                            DateBirth = person.DateBirth,
                            PhoneNumber = person.PhoneNumber,
                            Email = person.Email
                        });
                    }

                }

                return report;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Aniversariante - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Aniversariante - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Aniversariante - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Aniversariante - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Aniversariante - Dados vazios.");
            }
        }

        public async Task<TithePayerLaunchDTO> GetTithePayersLauchingWithFilters(int code)
        {
            TithePayerLaunchDTO tithePayer = new TithePayerLaunchDTO();

            try
            {

                tithePayer = await GetTithePayersLauchingWithFiltersRepository(code);

                return tithePayer;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista Lançamento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Dados vazios.");
            }
        }

        public async Task<List<TithePayerLaunchDTO>> GetTithePayersLauchingWithFilters(string? name, int code)
        {
            List<TithePayerLaunchDTO> tithePayer = new List<TithePayerLaunchDTO>();

            try
            {

                tithePayer = await GetTithePayersLauchingWithFiltersRepository(name, code);

                return tithePayer;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista Lançamento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Dados vazios.");
            }
        }

        public async Task<bool> UpdateTithePayer(TithePayerDTO tithePayer)
        {
            bool tithePayerWasUpdated = false;

            try
            {

                TithePayer tithePayerExists = await GetTithePayerById(tithePayer.TithePayerId);

                if (tithePayerExists == null || tithePayerExists.TithePayerId == 0)
                    throw new ValidationException("Dizimista não existente");

                tithePayer.UpdatedAt = DateTime.Now;

                byte[]? imageBytes = null;
                string? extension = null;

                if (tithePayer.TermFile != null && tithePayer.TermFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await tithePayer.TermFile.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();

                    extension = Path.GetExtension(tithePayer.TermFile.FileName);

                }

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
                    ZipCode = tithePayer.ZipCode.Replace("-",""),
                    Neighborhood = tithePayer.Neighborhood,
                    Complement = tithePayer.Complement,
                    CreatedAt = tithePayerExists.CreatedAt,
                    UpdatedAt = tithePayer.UpdatedAt,
                    TermFile = imageBytes == null ? tithePayerExists.TermFile : imageBytes,
                    Extension = extension
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

        public async Task<List<string>> GetAllAddressOfTithePayers()
        {
            List<string> address = new List<string>();

            try
            {

                address = await GetAllAddressOfTithePayersRepository();

                return address;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Endereços - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Endereços - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Endereços - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Endereços - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Endereços - Dados vazios.");
            }
        }

        public async Task<List<ReportNeighborhood>> GetReportTithePayerPerNeighborhood(string? name, string? address)
        {
            List<ReportNeighborhood> reportNeighborhoods = new List<ReportNeighborhood>();

            try
            {

                reportNeighborhoods = await GetReportTithePayerPerNeighborhoodRepository(name, address);

                return reportNeighborhoods;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Relatório Bairros - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Relatório Bairros - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Relatório Bairros - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Relatório Bairros - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Relatório Bairros - Dados vazios.");
            }
        }

        public async Task<List<ReportPaying>> GetPayingStatus(string? name, Status? status)
        {
            List<ReportPaying> reportPayings = new List<ReportPaying>();

            try
            {

                List<TithePayer> tithePayers = await GetTithePayersWithFilters(null, name);

                foreach (var person in tithePayers)
                {

                    List<TitheDTO> tithes = await _titheService.GetTithesByTithePayerId(person.TithePayerId);

                    if (tithes.Count == 0)
                    {
                        reportPayings.Add(new ReportPaying
                        {
                            TithePayerId = person.TithePayerId,
                            Name = person.Name,
                            StatusPaying = Status.NaoContribuinte,
                            PhoneNumber = person.PhoneNumber,
                            Email = person.Email,
                            LastContribuition = null,
                            AmountContribuition = 0
                        });
                    }
                    else
                    {

                        int amountMonths = CalculatePaidMonthsLastSemester(tithes); 

                        reportPayings.Add(new ReportPaying
                        {
                            TithePayerId = person.TithePayerId,
                            Name = person.Name,
                            StatusPaying = amountMonths == 0 ? Status.Inadimplente : Status.Adimplente,
                            PhoneNumber = person.PhoneNumber,
                            Email = person.Email,
                            LastContribuition = tithes.Max(x => x.PaymentMonth),
                            AmountContribuition = amountMonths
                        });
                    }

                }

                if(status != null)
                    reportPayings = reportPayings.Where(x => x.StatusPaying == status).ToList();

                return reportPayings;

            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Status Contribuintes - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Status Contribuintes - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Status Contribuintes - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Status Contribuintes - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Status Contribuintes - Dados vazios.");
            }
        }

        private int CalculatePaidMonthsLastSemester(List<TitheDTO> tithes)
        {
            int amountMonths = 0;

            DateTime actualMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);

            DateTime lastMonth = actualMonth.AddMonths(-6);

            foreach (var tithe in tithes)
            {
                DateTime titheDateFormatted = new DateTime(tithe.PaymentMonth.Year, tithe.PaymentMonth.Month, 1, 0, 0, 0);

                if (titheDateFormatted >= lastMonth && titheDateFormatted <= actualMonth)
                    amountMonths++;
            }

            return amountMonths;
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

        private async Task<TithePayerLaunchDTO> GetTithePayersLauchingWithFiltersRepository(int code)
        {
            TithePayerLaunchDTO tithePayer = new TithePayerLaunchDTO();

            try
            {
                tithePayer = await _tithePayerRepository.GetTithePayersLauchingWithFilters(code);

                return tithePayer;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista Lançamento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Dados vazios.");
            }
        }

        private async Task<List<TithePayerLaunchDTO>> GetTithePayersLauchingWithFiltersRepository(string? name, int code)
        {
            List<TithePayerLaunchDTO> tithePayer = new List<TithePayerLaunchDTO>();

            try
            {
                tithePayer = await _tithePayerRepository.GetTithePayersLauchingWithFilters(name, code);

                return tithePayer;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Dizimista Lançamento - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Dizimista Lançamento - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Dizimista Lançamento - Dados vazios.");
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

        private async Task<List<ReportTithePayer>> GetReportTithePayersRepository(string? paymentType, string? name, DateTime startPaymentDate, DateTime endPaymentDate)
        {
            List<ReportTithePayer> report = new List<ReportTithePayer>();

            try
            {
                report = await _tithePayerRepository.GetReportTithePayers(paymentType, name, startPaymentDate, endPaymentDate);

                return report;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Relatório Dizimista - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Relatório Dizimista - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Relatório Dizimista - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Relatório Dizimista - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Relatório Dizimista - Dados vazios.");
            }
        }

        private async Task<List<ReportBirthday>> GetReportTithePayersBirthdaysRepository(string? name, DateTime startBirthdayDate, DateTime endBirthdayDate)
        {
            List<ReportBirthday> report = new List<ReportBirthday>();

            try
            {
                report = await _tithePayerRepository.GetReportTithePayersBirthdays(name, startBirthdayDate, endBirthdayDate);

                return report;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Relatório Aniversariante - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Relatório Aniversariante - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Relatório Aniversariante - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Relatório Aniversariante - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Relatório Aniversariante - Dados vazios.");
            }
        }

        private async Task<List<string>> GetAllAddressOfTithePayersRepository()
        {
            List<string> address = new List<string>();

            try
            {
                address = await _tithePayerRepository.GetAllAddressOfTithePayers();

                return address;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Endereços - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Endereços - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Endereços - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Endereços - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Endereços - Dados vazios.");
            }
        }

        private async Task<List<ReportNeighborhood>> GetReportTithePayerPerNeighborhoodRepository(string? name, string? address)
        {
            List<ReportNeighborhood> reportNeighborhoods = new List<ReportNeighborhood>();

            try
            {
                reportNeighborhoods = await _tithePayerRepository.GetReportTithePayerPerNeighborhood(name, address);

                return reportNeighborhoods;
            }
            catch (DbException ex)
            {
                throw new RepositoryException("Consultar Relatório Bairros - Erro ao acessar o banco de dados.", ex);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ValidationException("Consultar Relatório Bairros - Estouro de limite.");
            }
            catch (FormatException)
            {
                throw new ValidationException("Consultar Relatório Bairros - Erro de formatação.");
            }
            catch (NullReferenceException)
            {
                throw new NullException("Consultar Relatório Bairros - Referência vazia.");
            }
            catch (ArgumentNullException)
            {
                throw new NullException("Consultar Relatório Bairros - Dados vazios.");
            }
        }


        #endregion

    }
}
