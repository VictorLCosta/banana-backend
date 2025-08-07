using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth.Jwt;

public class ConfigureJwtBearerOptions(IOptions<JwtOptions> jwtOptions, ILogger<ConfigureJwtBearerOptions> logger) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly ILogger<ConfigureJwtBearerOptions> _logger = logger;

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        byte[] key = Encoding.ASCII.GetBytes(_jwtOptions.Key);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateLifetime = true,
            LifetimeValidator = CustomLifetimeValidator,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context => 
            {
                _logger.LogError("Token invalido, expirado ou nao informado...");

                context.Response.StatusCode = 401;
                
                return Task.CompletedTask;
            },
            OnForbidden = _ => throw new Exception("You are not authorized to access this resource."),
        };
    }

    private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
    {
        return expires != null && expires > DateTime.UtcNow;
    }

}
