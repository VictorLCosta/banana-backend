namespace Application.Core.Identity.Tokens.Features;

public record RefreshTokenCommand(string Token, string RefreshToken);
