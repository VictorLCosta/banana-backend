using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.Core.Extensions;
using Application.Core.Identity.Tokens;
using Application.Core.Identity.Tokens.Features;

using Infrastructure.Auth.Jwt;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public class TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<TokenResponse>> GenerateTokenAsync(TokenGenerationCommand tokenRequest, string ipAddress, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(tokenRequest.Email.Trim().Normalize()) is not { } user
            || !await _userManager.CheckPasswordAsync(user, tokenRequest.Password))
        {
            return Result.Unauthorized();
        }

        if (!user.IsActive)
        {
            return Result.Unauthorized();
        }

        if (!user.EmailConfirmed)
        {
            return Result.Unauthorized();
        }

        return Result.Success(await GenerateTokensAndUpdateUser(user, ipAddress));
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenCommand refreshToken, string ipAddress)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(refreshToken.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user is null)
        {
            return Result.Unauthorized();
        }

        if (user.RefreshToken != refreshToken.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Unauthorized();
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string ipAddress)
    {
        string token = GenerateJwt(user, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user, string ipAddress) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.Now.AddMinutes(_jwtOptions.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress) =>
        [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new("fullName", $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new("ipAddress", ipAddress),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        ];
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception();
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}
