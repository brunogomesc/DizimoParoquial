using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface ITithePayerRepository
    {

        public Task<int> RegisterTithePayer(TithePayer tithePayer);

        public Task<TithePayer> GetTithePayerById(int id);

        public Task<List<TithePayer>> GetTithePayersWithFilters(string? document, string? name);

        public Task<TithePayerLaunchDTO> GetTithePayersLauchingWithFilters(int code);

        public Task<List<TithePayerLaunchDTO>> GetTithePayersLauchingWithFilters(string? name, int code);

        public Task<List<TithePayer>> GetTithePayersWithouthFilters();

        public Task<bool> UpdateTithePayer(TithePayer tithePayer);

        public Task<List<ReportTithePayer>> GetReportTithePayers(string? paymentType, string? name, DateTime startPaymentDate, DateTime endPaymentDate);

        public Task<List<ReportBirthday>> GetReportTithePayersBirthdays(string? name, DateTime startBirthdayDate, DateTime endBirthdayDate);

        public Task<List<string>> GetAllAddressOfTithePayers();

        public Task<List<ReportNeighborhood>> GetReportTithePayerPerNeighborhood(string? name, string? neighborhood);

    }
}
