using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MapleStoryMarketGraph.Data;
using MapleStoryMarketGraph.Models;
using BCrypt.Net;

namespace MapleStoryMarketGraph.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<(bool success, string message, User? user)> RegisterLocalUserAsync(string name, string email, string password, string passwordConfirm)
        {
            if (password != passwordConfirm)
                return (false, "비밀번호가 일치하지 않습니다.", null);

            if (await _dbContext.Users.AnyAsync(u => u.Email == email))
                return (false, "이미 가입된 이메일입니다.", null);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                AuthType = AuthType.Local,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return (true, "가입 성공!", user);
        }

        public async Task<(bool success, string message, string? token)> LoginLocalAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.AuthType == AuthType.Local);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return (false, "이메일 또는 비밀번호가 올바르지 않습니다.", null);

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, "이메일 또는 비밀번호가 올바르지 않습니다.", null);

            var token = GenerateJwtToken(user);
            return (true, "로그인 성공!", token);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtConfig = _configuration.GetSection("Authentication:Jwt");
            var keyStr = jwtConfig["Key"] ?? "VERY_SECRET_DEFAULT_KEY_THAT_MUST_BE_CHANGED";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("AuthType", user.AuthType.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"] ?? "MesoMarket",
                audience: jwtConfig["Issuer"] ?? "MesoMarket",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
