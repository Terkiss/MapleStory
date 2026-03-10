using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MapleStoryMarketGraph.Data;
using MapleStoryMarketGraph.Models;
using Google.Apis.Auth;

namespace MapleStoryMarketGraph.Services
{
    public class GoogleAuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(ApplicationDbContext dbContext, AuthService authService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<(bool success, string message, string? token)> AuthenticateWithGoogleAsync(string idToken)
        {
            try
            {
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                });

                if (payload == null)
                    return (false, "유효하지 않은 구글 토큰입니다.", null);

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.GoogleSubjectId == payload.Subject || u.Email == payload.Email);

                if (user == null)
                {
                    // New user from Google
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Name = payload.Name ?? "Google User",
                        Email = payload.Email,
                        GoogleSubjectId = payload.Subject,
                        AuthType = AuthType.Google,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();
                }
                else if (user.AuthType == AuthType.Local)
                {
                    // Existing local user links Google
                    user.GoogleSubjectId = payload.Subject;
                    user.AuthType = AuthType.Google; // Upgrade to Google if they prefer it? Or just keep both.
                    await _dbContext.SaveChangesAsync();
                }

                var token = _authService.GenerateJwtToken(user);
                return (true, "구글 로그인 성공!", token);
            }
            catch (Exception ex)
            {
                return (false, $"구글 인증 실패: {ex.Message}", null);
            }
        }
    }
}
