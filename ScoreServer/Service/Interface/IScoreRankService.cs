using ScoreServer.Model;

namespace ScoreServer.Service.Interface
{
    public interface IScoreRankService
    {
        Task<decimal> UpdateScore(long customerId, decimal score);
        Task<List<CustomerScore>> GetCustomersByRank(int? start, int? end);
        Task<List<CustomerScore>> GetCustomersByCustomerIdAndAroundRank(long customerId, int high = 0, int low = 0);
    }
}
