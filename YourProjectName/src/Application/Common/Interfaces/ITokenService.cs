// Application/Common/Interfaces/ITokenService.cs
using YourProjectName.Application.Common.Models;
using System.Threading.Tasks;

namespace YourProjectName.Application.Common.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Génère un access + refresh token pour l'utilisateur donné (login par username).
    /// </summary>
    Task<(TokenModel AccessToken, TokenModel RefreshToken)> GenerateTokensAsync(string username);

    /// <summary>
    /// Tente de rafraîchir les tokens à partir d'un refresh token valide (rotation).
    /// </summary>
    /// <returns>Tuple de nouveaux tokens, ou null si refresh invalide.</returns>
    Task<(TokenModel AccessToken, TokenModel RefreshToken)?> RefreshTokensAsync(string refreshToken);
}
