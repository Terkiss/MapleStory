using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MapleStoryMarketGraph.Services;
using MapleStoryMarketGraph.Models;
using MapleStoryMarketGraph.Data;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;

namespace MapleStoryMarketGraph.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly NexonApiService _nexonApiService;

        public UserController(ApplicationDbContext dbContext, NexonApiService nexonApiService)
        {
            _dbContext = dbContext;
            _nexonApiService = nexonApiService;
        }

        [HttpPost("me/nexon-key")]
        public async Task<IActionResult> RegisterNexonKey([FromBody] NexonApiKeyRequest request)
        {
            var isValid = await _nexonApiService.VerifyApiKeyAsync(request.ApiKey);
            if (!isValid)
                return BadRequest(new { message = "유효하지 않은 넥슨 API Key입니다." });

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { message = "사용자 인증 실패" });

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "사용자를 찾을 수 없습니다." });

            user.NexonApiKey = request.ApiKey;
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "넥슨 API Key 등록 성공!" });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new { message = "사용자 인증 실패" });

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "사용자를 찾을 수 없습니다." });

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.AuthType,
                HasNexonApiKey = !string.IsNullOrEmpty(user.NexonApiKey)
            });
        }
    }
}
