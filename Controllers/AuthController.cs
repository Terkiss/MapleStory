using Microsoft.AspNetCore.Mvc;
using MapleStoryMarketGraph.Services;
using MapleStoryMarketGraph.Models;
using System.Threading.Tasks;

namespace MapleStoryMarketGraph.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly GoogleAuthService _googleAuthService;

        public AuthController(AuthService authService, GoogleAuthService googleAuthService)
        {
            _authService = authService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("register/local")]
        public async Task<IActionResult> RegisterLocal([FromBody] RegisterRequest request)
        {
            var (success, message, user) = await _authService.RegisterLocalUserAsync(request.Name, request.Email, request.Password, request.PasswordConfirm);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPost("login/local")]
        public async Task<IActionResult> LoginLocal([FromBody] LoginRequest request)
        {
            var (success, message, token) = await _authService.LoginLocalAsync(request.Email, request.Password);
            if (!success)
                return Unauthorized(new { message });

            return Ok(new { message, token });
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> LoginGoogle([FromBody] GoogleLoginRequest request)
        {
            var (success, message, token) = await _googleAuthService.AuthenticateWithGoogleAsync(request.IdToken);
            if (!success)
                return Unauthorized(new { message });

            return Ok(new { message, token });
        }
    }
}
