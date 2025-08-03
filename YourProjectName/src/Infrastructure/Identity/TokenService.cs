using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Application.Common.Models;
using YourProjectName.Domain.Entities;

namespace YourProjectName.Infrastructure.Identity;

public class TokenService : ITokenService
{
  private readonly IApplicationDbContext _context;
  private readonly IMemoryCache _cache;
  private readonly JwtSecurityTokenHandler _jwtHandler;
  private readonly byte[] _signingKey;
  private static readonly TimeSpan AccessTokenTTL = TimeSpan.FromHours(1);
  private static readonly TimeSpan RefreshTokenTTL = TimeSpan.FromDays(1);

  public TokenService(
    IApplicationDbContext context,
    IMemoryCache cache,
    IConfiguration config)
  {
    _context = context;
    _cache = cache;
    _jwtHandler = new JwtSecurityTokenHandler();
    _signingKey = Encoding.UTF8.GetBytes(config["Jwt:SigningKey"]
      ?? throw new ArgumentNullException("Jwt:SigningKey"));
  }

  public async Task<(TokenModel AccessToken, TokenModel RefreshToken)> GenerateTokensAsync(string username)
  {
    var user = await GetUserFromUsernameAsync(username)
      ?? throw new InvalidOperationException($"User '{username}' not found.");

    var access = GenerateAccessToken(user);
    var refresh = GenerateRefreshTokenModel();

    // Store the refresh→username mapping in the cache
    _cache.Set(GetCacheKey(refresh.Value), user.Username,
      new MemoryCacheEntryOptions { AbsoluteExpiration = refresh.Expiration });

    return (access, refresh);
  }

  public async Task<(TokenModel AccessToken, TokenModel RefreshToken)?> RefreshTokensAsync(string refreshToken)
  {
    var cacheKey = GetCacheKey(refreshToken);
    if (!_cache.TryGetValue(cacheKey, out string? username) || username is null)
      return null;

    // Remove the old token
    _cache.Remove(cacheKey);

    // Generate new tokens
    return await GenerateTokensAsync(username);
  }

  // ——— Private Methods ———

  private async Task<User?> GetUserFromUsernameAsync(string username)
    => await _context.Users
      .AsNoTracking()
      .FirstOrDefaultAsync(u => u.Username == username);

  private TokenModel GenerateAccessToken(User user)
  {
    var now = DateTimeOffset.UtcNow;
    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(ClaimTypes.Name, user.Username),
      new Claim(ClaimTypes.Role, user.Role.ToString())
    };
    var creds = new SigningCredentials(
      new SymmetricSecurityKey(_signingKey),
      SecurityAlgorithms.HmacSha256);

    var jwt = new JwtSecurityToken(
      claims: claims,
      notBefore: now.UtcDateTime,
      expires: now.Add(AccessTokenTTL).UtcDateTime,
      signingCredentials: creds);

    var tokenValue = _jwtHandler.WriteToken(jwt);
    return new TokenModel(tokenValue, now.Add(AccessTokenTTL));
  }

  private TokenModel GenerateRefreshTokenModel()
  {
    var now = DateTimeOffset.UtcNow;
    var value = Guid.NewGuid().ToString("N");
    return new TokenModel(value, now.Add(RefreshTokenTTL));
  }

  private static string GetCacheKey(string refreshToken)
    => $"refresh-token:{refreshToken}";
}
