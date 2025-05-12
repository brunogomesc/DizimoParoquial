using DizimoParoquial.Models;

namespace DizimoParoquial.Data.Interface
{
    public interface IIncomeRepository
    {

        public Task<int> SaveIncome(Income income);

        public Task<List<ReportSum>> GetReportSum(string? paymentType, DateTime startPaymentDate, DateTime endPaymentDate);

    }
}
