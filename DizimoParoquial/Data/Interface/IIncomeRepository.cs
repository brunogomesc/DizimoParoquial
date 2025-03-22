using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IIncomeRepository
    {

        public Task<int> SaveIncome(Income income);

    }
}
