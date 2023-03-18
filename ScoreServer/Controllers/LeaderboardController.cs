using Microsoft.AspNetCore.Mvc;
using ScoreServer.Service.Interface;

namespace LeaderboardService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly IScoreRankService _scoreService;

        public LeaderboardController(IScoreRankService scoreService)
        {
            _scoreService = scoreService;
        }

        /// <summary>
        /// get customers by rank
        /// </summary>
        /// <param name="start">start rank</param>
        /// <param name="end">end rank</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomersByRank(int? start, int? end)
        {
            var result = await _scoreService.GetCustomersByRank(start, end);
            return Ok(result);
        }

        /// <summary>
        /// get customers by customerId and Around Rank
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="high">out of customer rank</param>
        /// <param name="low">below customer rank</param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomersByCustomerIdAndAroundRank(long customerId, int high = 0, int low = 0)
        {
            var result = await _scoreService.GetCustomersByCustomerIdAndAroundRank(customerId, high, low);
            return Ok(result);
        }
    }
}
