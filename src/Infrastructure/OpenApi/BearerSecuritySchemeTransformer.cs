using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Infrastructure.OpenApi;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _schemes;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider schemes)
    {
        _schemes = schemes;
    }

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken = default)
    {
        // opcional: verifica se o esquema "Bearer" está registrado (útil se você tiver vários schemes)
        var all = await _schemes.GetAllSchemesAsync();
        if (!all.Any(s => s.Name?.Contains("Bearer", StringComparison.OrdinalIgnoreCase) == true))
        {
            // mesmo que não exista explicitamente, nós adicionamos o esquema no OpenAPI para que Scalar saiba lidar
        }

        // adiciona o security scheme Bearer (components)
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        var schemeName = "Bearer";
        if (!document.Components.SecuritySchemes.ContainsKey(schemeName))
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Informe: 'Bearer {seu token JWT}'",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = schemeName }
            };

            document.Components.SecuritySchemes[schemeName] = bearerScheme;

            // adiciona requirement global (aplica a todos os endpoints)
            var requirement = new OpenApiSecurityRequirement
            {
                [ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = schemeName } } ] = Array.Empty<string>()
            };

            document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
            document.SecurityRequirements.Add(requirement);
        }
    }
}
