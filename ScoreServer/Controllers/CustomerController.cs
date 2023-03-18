using Microsoft.AspNetCore.Mvc;
using ScoreServer.Service.Interface;

namespace LeaderboardService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IScoreRankService _scoreService;

        public CustomerController(IScoreRankService scoreService)
        {
            _scoreService = scoreService;
        }

        /// <summary>
        /// Add or update a customer's score
        /// </summary>
        /// <param name="customerId">customer id</param>
        /// <param name="score">score</param>
        /// <returns></returns>
        [HttpPost("{customerId}/score/{score}")]
        public async Task<IActionResult> UpdateScore(long customerId, decimal score)
        {
            var result = await _scoreService.UpdateScore(customerId, score);
            return Ok(result);
        }
    }
}
