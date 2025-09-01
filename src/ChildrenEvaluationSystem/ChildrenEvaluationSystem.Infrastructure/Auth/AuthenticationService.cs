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
    public Task LoginAsync(string? returnUrl = "/")
    {
        var target = BuildLocalReturnUrl(returnUrl);
        var signInUrl = $"/MicrosoftIdentity/Account/SignIn?returnUrl={Uri.EscapeDataString(target)}";
        navigation.NavigateTo(signInUrl, forceLoad: true);
        return Task.CompletedTask;
    }

    public Task LogoutAsync(string? returnUrl = "/")
    {
        var target = BuildLocalReturnUrl(returnUrl);
        var signOutUrl = $"/MicrosoftIdentity/Account/SignOut?returnUrl={Uri.EscapeDataString(target)}";
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

    private static string BuildLocalReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl)) return "/";
        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out _)) return "/";
        if (!returnUrl.StartsWith("/")) return "/";
        return returnUrl;
    }
}