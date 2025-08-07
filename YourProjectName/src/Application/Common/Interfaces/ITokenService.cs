using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourProjectName.Application.Common.Models;

namespace YourProjectName.Application.Common.Interfaces
{
    public interface ITokenService
    {
        public TokenModel GenerateAccessTokenFromUsername(string username);
        public TokenModel GenerateRefreshTokenFromUsername(string username);
        public TokenModel ConsumeRefreshToken(string username, string refreshTokenValue);
    }
}
