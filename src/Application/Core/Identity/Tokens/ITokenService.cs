using Application.Core.Identity.Tokens.Features;

namespace Application.Core.Identity.Tokens;

public interface ITokenService : IService
{
    public Task<Result<TokenResponse>> GenerateTokenAsync(TokenGenerationCommand tokenRequest, string ipAddress, CancellationToken cancellationToken);

    public Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenCommand refreshToken, string ipAddress);
}
