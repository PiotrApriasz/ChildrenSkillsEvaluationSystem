using System.Security.Claims;
using ChildrenEvaluationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using IAuthenticationService = ChildrenEvaluationSystem.Application.Interfaces.IAuthenticationService;

namespace ChildrenEvaluationSystem.Infrastructure.Auth;

public class AuthenticationService(IHttpContextAccessor httpContextAccessor, NavigationManager navigation) : IAuthenticationService
{
    private const string Fallback = "/Home";
    
    public Task LoginAsync(string? returnUrl = "/")
    {
        var target = BuildLocalReturnUrl(returnUrl);
        var signInUrl = $"/MicrosoftIdentity/Account/SignIn?redirectUri={Uri.EscapeDataString(target)}";
        navigation.NavigateTo(signInUrl, forceLoad: true);
        return Task.CompletedTask;
    }

    public Task LogoutAsync(string? returnUrl = "/")
    {
        var target = BuildLocalReturnUrl(returnUrl);
        var signOutUrl = $"/MicrosoftIdentity/Account/SignOut?postLogoutRedirectUri={Uri.EscapeDataString(target)}";
        navigation.NavigateTo(signOutUrl, forceLoad: true);
        return Task.CompletedTask;
    }

    public bool IsAuthenticatedAsync()
        => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public ClaimsIdentity? GetUserIdentityAsClaims()
        => httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;

    public string GetUsername()
        => GetUserIdentityAsClaims()?.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown User";

    public string GetEmail()
    {
        var identity = GetUserIdentityAsClaims();
        if (identity == null) return "Unavailable";
        
        var email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                    ?? identity.Claims.FirstOrDefault(c => c.Type.Equals("emails", StringComparison.OrdinalIgnoreCase))?.Value
                    ?? identity.Claims.FirstOrDefault(c => c.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;

        return string.IsNullOrWhiteSpace(email) ? "Unknown Email" : email!;
    }

    public string GetFirstName()
        => GetUserIdentityAsClaims()?.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "Unknown First Name";

    public string GetLastName()
        => GetUserIdentityAsClaims()?.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "Unknown Last Name";

    private string BuildLocalReturnUrl(string? returnUrl, string fallback = Fallback)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return fallback;

        returnUrl = Uri.UnescapeDataString(returnUrl).Trim();
        
        var hashIdx = returnUrl.IndexOf('#');
        if (hashIdx >= 0)
            returnUrl = returnUrl[..hashIdx];
        
        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var abs))
        {
            var req = httpContextAccessor.HttpContext?.Request;
            if (req == null) return fallback;

            var sameHost = string.Equals(abs.Host, req.Host.Host, StringComparison.OrdinalIgnoreCase);
            var sameScheme = string.Equals(abs.Scheme, req.Scheme, StringComparison.OrdinalIgnoreCase);
            if (!sameHost || !sameScheme) return fallback;
            
            var pathAndQuery = abs.PathAndQuery; 
            return string.IsNullOrEmpty(pathAndQuery) ? fallback : pathAndQuery;
        }
        
        if (!returnUrl.StartsWith('/') || returnUrl.StartsWith("//")) return fallback;

        return returnUrl;
    }
}