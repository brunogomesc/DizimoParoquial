using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface ITitheRepository
    {

        public Task<bool> SaveTithes(List<Tithe> tithes);

        public Task<bool> UpdateTithe(Tithe tithe);

        public Task<List<TithePayerLaunchDTO>> GetTithesWithFilters(string? name, int tithePayerCode, string? document);

        public Task<List<TitheDTO>> GetTithesByTithePayerId(int tithePayerId);

        public Task<List<TitheDTO>> GetReportTithesMonth(string paymentType, string name, DateTime startPaymentDate, DateTime endPaymentDate);

        public Task<TitheDTO> GetTitheByTitheId(int id);

    }
}
