using ScoreServer.Service.Interface;
using ScoreServer.Model;
using ScoreServer.Rank;
using System.Collections.Concurrent;

namespace ScoreServer.Service
{
    public class ScoreRankService : IScoreRankService
    {
        private readonly ConcurrentDictionary<long, decimal> _customerScoreDic = new();

        private readonly SortedSet<CustomerScore> _leaderBoard = new(new CustomerScoreComparer());

        private int _lock = 0;

        private const decimal _minScore = -1000;
        private const decimal _maxScore = 1000;

        public ScoreRankService()
        {
        }

        public async Task<decimal> UpdateScore(long customerId, decimal score)
        {
            if (customerId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(customerId), $"{nameof(customerId)} must be a positive number");
            }

            if (score < _minScore || score > _maxScore)
            {
                throw new ArgumentOutOfRangeException(nameof(score), $"Scores range from {_minScore} to {_maxScore}");
            }

            int retryCount = 10;
            while (true)
            {
                bool isUpdateScoreSuccess;
                if (_customerScoreDic.TryGetValue(customerId, out decimal tempScore))
                {
                    score = tempScore + score;
                    isUpdateScoreSuccess = _customerScoreDic.TryUpdate(customerId, score, tempScore);
                }
                else
                {
                    isUpdateScoreSuccess = _customerScoreDic.TryAdd(customerId, score);
                }

                if (isUpdateScoreSuccess)
                {
                    UpdateLeaderBoard(customerId, score, tempScore);

                    return score;
                }

                retryCount--;
                if (retryCount <= 0)
                {
                    throw new BadHttpRequestException("update score failed, please retry");
                }
                await Task.Delay(10);
            }
        }

        private void UpdateLeaderBoard(long customerId, decimal score, decimal beforeScore)
        {
            try
            {
                while (Interlocked.CompareExchange(ref _lock, 1, 0) == 1)
                {
                    Task.Delay(1);
                }

                var existingCustomer = _leaderBoard.FirstOrDefault(c => c.CustomerId == customerId);
                if (existingCustomer != null)
                {
                    _leaderBoard.Remove(existingCustomer);
                }
                if (score > 0)
                {
                    _leaderBoard.Add(new CustomerScore(customerId, score));
                }

                int rank = 1;
                foreach (var customer in _leaderBoard)
                {
                    customer.Rank = rank;
                    rank++;
                }
                Interlocked.Exchange(ref _lock, 0);
            }
            catch (Exception ex)
            {
                _customerScoreDic.TryUpdate(customerId, beforeScore, score);
                throw new Exception($"Update leaderboard error: {ex.Message}");
            }
        }

        public Task<List<CustomerScore>> GetCustomersByRank(int? start, int? end)
        {
            var count = _leaderBoard.Count;
            if (!start.HasValue || start < 1) start = 1;
            if (!end.HasValue || end > count) end = count;
            if (start > end) return Task.FromResult(new List<CustomerScore>());
            return Task.FromResult(_leaderBoard.Skip(start.Value - 1).Take(end.Value - start.Value + 1).ToList());
        }

        public Task<List<CustomerScore>> GetCustomersByCustomerIdAndAroundRank(long customerId, int aboveRank = 0, int belowRank = 0)
        {
            if (customerId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(customerId), $"{nameof(customerId)} must be a positive number");
            }

            var customer = _leaderBoard.SingleOrDefault(c => c.CustomerId == customerId) ?? throw new InvalidOperationException($"count not find customer, customerId: {customerId}");
            var count = _leaderBoard.Count;
            var rank = customer.Rank;

            var startRank = rank - aboveRank;
            if (startRank < 1) startRank = 1;

            var endRank = rank + belowRank;
            if (endRank > count) endRank = count;

            if (startRank > endRank) return Task.FromResult(new List<CustomerScore>());

            return Task.FromResult(_leaderBoard.Skip(startRank - 1).Take(endRank - startRank + 1).ToList());
        }

    }
}
