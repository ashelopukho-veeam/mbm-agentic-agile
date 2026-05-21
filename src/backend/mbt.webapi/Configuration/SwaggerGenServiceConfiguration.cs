using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace mbt.webapi.Configuration;

public static class SwaggerGenServiceConfiguration
{
    public static void AddSwaggerGenService(this IServiceCollection services, AzureConfigOptions azureConfigOptions)
    {
        var authorizationUrl = azureConfigOptions.Instance + azureConfigOptions.TenantId + "/oauth2/v2.0/authorize";
        var tokenUrl = azureConfigOptions.Instance + azureConfigOptions.TenantId + "/oauth2/v2.0/token";
        var scopeName = "api://" + azureConfigOptions.ClientId + "/access_as_user";
        const string scopeDescription = "Access as user";

        services.AddSwaggerGenService(authorizationUrl, tokenUrl, scopeName, scopeDescription);
    }

    private static void AddSwaggerGenService(this IServiceCollection services,
        string authorizationUrl, string tokenUrl, string scopeName, string scopeDescription)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "t", Version = "v1" });
            c.EnableAnnotations();
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl =
                            new Uri(authorizationUrl
                            ),
                        TokenUrl = new Uri(
                            tokenUrl),
                        Scopes = new Dictionary<string, string>
                        {
                            {
                                scopeName,
                                scopeDescription
                            }
                        }
                    }
                }
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void UseSwaggerUiConfig(this WebApplication app, string clientId, string clientSecret)
    {
        var swaggerConfigSection = new SwaggerConfigOptions();
        app.Configuration.GetSection(SwaggerConfigOptions.SectionName).Bind(swaggerConfigSection);

        if (!swaggerConfigSection.IsEnabled) return;

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "t v1");
            c.OAuthClientId(clientId);
            c.OAuthClientSecret(clientSecret);
            c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        });
    }
}
