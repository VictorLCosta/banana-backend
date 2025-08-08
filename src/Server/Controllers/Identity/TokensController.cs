using Application.Core.Identity.Tokens;
using Application.Core.Identity.Tokens.Features;

using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers.Identity;

public class TokensController(ITokenService tokenService) : BaseApiController
{
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<Result<TokenResponse>> GetTokenAsync(TokenGenerationCommand request, CancellationToken cancellationToken)
    {
        return await _tokenService.GenerateTokenAsync(request, GetIpAddress()!, cancellationToken);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<Result<TokenResponse>> RefreshAsync(RefreshTokenCommand request)
    {
        return await _tokenService.RefreshTokenAsync(request, GetIpAddress()!);
    }

    private string? GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}
