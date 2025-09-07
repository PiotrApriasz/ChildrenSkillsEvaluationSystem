using System.Text.Json;
using ChildrenEvaluationSystem.Application.Interfaces;
using ChildrenEvaluationSystem.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

namespace ChildrenEvaluationSystem.Infrastructure;

public static class InfrastructureServiceRegister
{
    public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration cfgSection)
    {
        var options = new CosmosOptions();
        cfgSection.Bind(options);
        services.AddSingleton(options);

        services.AddSingleton(sp =>
        {
            var clientOptions = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };
            return new CosmosClient(options.Account, options.Key, clientOptions);
        });

        services.AddSingleton<ICosmosContainerProvider, CosmosContainerProvider>();

        var jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        jsonOpts.Converters.Add(new DateOnlyJsonConverter());
        services.AddSingleton(jsonOpts);

        services.AddScoped(typeof(IRepository<>), typeof(CosmosRepository<>));

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, 
        IConfiguration cfgSection, string? signedOutRedirectUri)
    {
        services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(cfgSection);

        services.PostConfigure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.SaveTokens = true;

            options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
            {
                var idToken = await ctx.HttpContext.GetTokenAsync("id_token");
                if (!string.IsNullOrEmpty(idToken))
                    ctx.ProtocolMessage.IdTokenHint = idToken;
                
                var loginHintClaim = ctx.HttpContext.User.FindFirst("login_hint");
                if (loginHintClaim != null && !string.IsNullOrEmpty(loginHintClaim.Value))
                    ctx.ProtocolMessage.SetParameter("logout_hint", loginHintClaim.Value);
                
                if (string.IsNullOrEmpty(ctx.ProtocolMessage.ClientId))
                    ctx.ProtocolMessage.ClientId = options.ClientId;
                
                var authority = options.Authority?.TrimEnd('/') ?? "";
                var baseAuth  = authority.EndsWith("/v2.0", StringComparison.OrdinalIgnoreCase)
                    ? authority[..^"/v2.0".Length]
                    : authority;
                ctx.ProtocolMessage.IssuerAddress = $"{baseAuth}/oauth2/v2.0/logout";
                
                var postLogout = signedOutRedirectUri ?? "/";
                ctx.ProtocolMessage.PostLogoutRedirectUri =
                    $"{ctx.Request.Scheme}://{ctx.Request.Host}{postLogout}";
            };
            
            options.Scope.Add("openid");
            options.Scope.Add("profile");
        });
        
        services.Configure<CookieAuthenticationOptions>(
            CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "ChildrenEvaluationSystem.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.SlidingExpiration = true;
        });

        IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        IdentityModelEventSource.ShowPII = true;
        
        services.AddAuthorization();
        services.AddCascadingAuthenticationState();
        
        return services;
    }
}