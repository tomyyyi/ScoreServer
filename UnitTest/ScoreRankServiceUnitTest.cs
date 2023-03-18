using ScoreServer.Service;
using ScoreServer.Service.Interface;
using Xunit;

namespace ScoreServerUnitTest
{
    public class ScoreRankServiceUnitTest
    {
        private readonly IScoreRankService _scoreRankService;

        private const decimal _minScore = -1000;
        private const decimal _maxScore = 1000;

        public ScoreRankServiceUnitTest()
        {
            _scoreRankService = new ScoreRankService();
        }

        [Fact]
        public async Task UpdateScoreAndCustomerIdIsNotPositive()
        {
            try
            {
                var customerId = -15514665;
                var score = 1;
                var result = await _scoreRankService.UpdateScore(customerId, score);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentOutOfRangeException);
                Assert.Contains($"customerId must be a positive number", e.Message);
            }
        }

        [Fact]
        public async Task UpdateScoreAndScoreIsLessThanMinValue()
        {
            try
            {
                var customerId = 15514665;
                var score = _minScore - 1;
                var result = await _scoreRankService.UpdateScore(customerId, score);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentOutOfRangeException);
                Assert.Contains($"Scores range from {_minScore} to {_maxScore}", e.Message);
            }
        }

        [Fact]
        public async Task UpdateScoreAndScoreIsMoreThanMaxValue()
        {
            try
            {
                var customerId = 15514665;
                var score = _maxScore + 1;
                var result = await _scoreRankService.UpdateScore(customerId, score);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentOutOfRangeException);
                Assert.Contains($"Scores range from {_minScore} to {_maxScore}", e.Message);
            }
        }

        [Fact]
        public async Task UpdateScoreSuccess()
        {
            var customerId = 15514665;
            var score = 124;
            var result = await _scoreRankService.UpdateScore(customerId, score);
            Assert.Equal(result, score);
        }

        [Fact]
        public async Task UpdateScoreTwiceSuccess()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.UpdateScore(customerId, score);
            Assert.Equal(result, score + score);
        }


        [Fact]
        public async Task GetCustomersByRankEmpty()
        {
            var result = await _scoreRankService.GetCustomersByRank(null, null);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCustomersByRankEmpty2()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByRank(10, null);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCustomersByRankEmpty3()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByRank(10, 1);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCustomersByRankSuccess()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByRank(null, null);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCustomersByRankSuccess2()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByRank(1, 10);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCustomersByRankSuccess3()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            customerId++;
            score--;
            await _scoreRankService.UpdateScore(customerId, score);

            var result = await _scoreRankService.GetCustomersByRank(1, 10);
            Assert.Equal(2, result.Count);
        }


        [Fact]
        public async Task GetCustomersByCustomerIdEmpty()
        {
            var customerId = 15514665;
            try
            {
                var result = await _scoreRankService.GetCustomersByCustomerIdAndAroundRank(customerId);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentNullException);
                Assert.Contains($"count not find customer, customerId: {customerId}", e.Message);
            }
        }

        [Fact]
        public async Task GetCustomersByCustomerIdEmpty2()
        {
            var customerId = -15514665;
            try
            {
                var result = await _scoreRankService.GetCustomersByCustomerIdAndAroundRank(customerId);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentOutOfRangeException);
                Assert.Contains($"customerId must be a positive number", e.Message);
            }
        }

        [Fact]
        public async Task GetCustomersByCustomerIdSuccess()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByCustomerIdAndAroundRank(customerId);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCustomersByCustomerIdSuccess2()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByCustomerIdAndAroundRank(customerId, 1, 1);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCustomersByCustomerIdSuccess3()
        {
            var customerId = 15514665;
            var score = 124;
            await _scoreRankService.UpdateScore(customerId, score);
            customerId++;
            score--;
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByCustomerIdAndAroundRank(customerId, 1, 1);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCustomerRankScore()
        {
            var customerId = 1;
            var score = 1;
            await _scoreRankService.UpdateScore(customerId, score);
            await _scoreRankService.UpdateScore(customerId, score);
            await _scoreRankService.UpdateScore(customerId, score);
            var result = await _scoreRankService.GetCustomersByCustomerIdAndAroundRank(customerId);
            Assert.Equal(3, result.First().Score);
        }

    }
}