using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Application.Common.Models;

namespace YourProjectName.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        // Constants
        private static readonly TimeSpan _accessTokenDuration = TimeSpan.FromMinutes(60);
        private static readonly TimeSpan _refreshTokenDuration = TimeSpan.FromDays(1);

        // Services
        private readonly IMemoryCache _cache;

        // Configuration
        private readonly SigningCredentials _signinCredentials;
        private readonly MemoryCacheEntryOptions _refreshTokenCacheOptions;

        public TokenService(IMemoryCache cache, IConfiguration config)
        {
            // Services
            _cache = cache;

            // Validation
            string jwtKey = config["Jwt:JwtKey"] ?? throw new InvalidOperationException("JwtKey configuration is missing or empty");

            // Configuration
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            _signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            
            // Cache options for refresh tokens
            _refreshTokenCacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = _refreshTokenDuration,
                Priority = CacheItemPriority.Normal
            };
        }

        private static string GetRefreshTokenCacheKey(string refreshTokenValue)
        {
            return $"refresh_token_{refreshTokenValue}";
        }

        public TokenModel GenerateAccessTokenFromUsername(string username)
        {
            // Setup token
            List<Claim> claims = new()
                {
                    new(ClaimTypes.UserData, JsonSerializer.Serialize(username)),
                };
            JwtSecurityToken tokenOptions = new(
                issuer: "http://monitorix",
                audience: "http://monitorix",
                expires: DateTime.Now.AddSeconds(_accessTokenDuration.TotalSeconds),
                signingCredentials: _signinCredentials,
                claims: claims
            );

            // Create the access token object
            return new TokenModel(
                new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                DateTime.Now.AddSeconds(_accessTokenDuration.TotalSeconds)
            );
        }

        public TokenModel GenerateRefreshTokenFromUsername(string username)
        {
            // Generate a random value on 1024 bits
            var randomBytes = new byte[128];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            string refreshTokenValue = Convert.ToBase64String(randomBytes);

            // Create the refresh token object
            TokenModel refreshToken = new(
                refreshTokenValue,
                DateTime.Now.AddSeconds(_refreshTokenDuration.TotalSeconds)
            );

            // Store the username associated with this refresh token in cache
            string cacheKey = GetRefreshTokenCacheKey(refreshTokenValue);
            _cache.Set(cacheKey, username, _refreshTokenCacheOptions);

            // Return the refresh token
            return refreshToken;
        }

        public TokenModel ConsumeRefreshToken(string username, string refreshTokenValue)
        {
            // Check if the refresh token exists in cache and is associated with the correct user
            string cacheKey = GetRefreshTokenCacheKey(refreshTokenValue);
            if (_cache.TryGetValue(cacheKey, out string? cachedUsername) &&
                cachedUsername == username)
            {
                _cache.Remove(cacheKey);
                return GenerateAccessTokenFromUsername(username);
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }
        }
    }
}
