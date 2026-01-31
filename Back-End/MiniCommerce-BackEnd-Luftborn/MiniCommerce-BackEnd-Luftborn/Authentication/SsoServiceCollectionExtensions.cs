using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace MiniCommerce_BackEnd_Luftborn.Authentication;

public static class SsoServiceCollectionExtensions
{
    public static IServiceCollection AddSsoAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var sso = configuration.GetSection(SsoConfiguration.SectionName).Get<SsoConfiguration>() ?? new SsoConfiguration();

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAssertion(ctx =>
                {
                    if (!sso.Enabled) return true;
                    return ctx.User.Identity?.IsAuthenticated == true;
                })
                .Build();
        });

        if (!sso.Enabled) return services;

        var authority = sso.Authority?.TrimEnd('/');
        var validIssuers = new[] { authority, authority + "/" };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = sso.Audience;
                options.RequireHttpsMetadata = sso.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers = validIssuers,
                    ValidateAudience = !string.IsNullOrEmpty(sso.Audience),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
                options.MetadataAddress = !string.IsNullOrEmpty(sso.MetadataAddress)
                    ? sso.MetadataAddress
                    : null;
            });

        return services;
    }
}
