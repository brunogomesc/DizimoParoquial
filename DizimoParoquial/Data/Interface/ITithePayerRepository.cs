using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface ITithePayerRepository
    {

        public Task<int> RegisterTithePayer(TithePayer tithePayer);

        public Task<TithePayer> GetTithePayerById(int id);

        public Task<List<TithePayer>> GetTithePayersWithFilters(string? document, string? name);

        public Task<TithePayerLaunchDTO> GetTithePayersLauchingWithFilters(string? name, int code);

        public Task<List<TithePayer>> GetTithePayersWithouthFilters();

        public Task<bool> UpdateTithePayer(TithePayer tithePayer);

    }
}
