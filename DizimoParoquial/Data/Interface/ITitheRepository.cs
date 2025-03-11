using DizimoParoquial.DTOs;
using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface ITitheRepository
    {

        public Task<bool> SaveTithes(List<Tithe> tithes);

        public Task<List<TitheDTO>> GetTithesWithFilters(string? name, int tithePayerCode);

    }
}
