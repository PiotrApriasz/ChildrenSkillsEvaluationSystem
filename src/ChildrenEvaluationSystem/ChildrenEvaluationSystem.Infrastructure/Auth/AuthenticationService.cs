using System.Security.Claims;
using ChildrenEvaluationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using IAuthenticationService = ChildrenEvaluationSystem.Application.Interfaces.IAuthenticationService;

namespace ChildrenEvaluationSystem.Infrastructure.Auth;

public class AuthenticationService(IHttpContextAccessor httpContextAccessor) : IAuthenticationService
{
    public async Task LoginAsync()
    {
        var context = httpContextAccessor.HttpContext;
        if (context != null)
        {
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }

    public async Task LogoutAsync()
    {
        var context = httpContextAccessor.HttpContext;
        if (context != null)
        {
            await context.SignOutAsync();
        }
    }

    public bool IsAuthenticatedAsync()
    {
        return httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public ClaimsIdentity? GetUserIdentityAsClaims()
    {
        return httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
    }

    public string GetUsername()
    {
        var identity = GetUserIdentityAsClaims();
        if (identity != null)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown User";   
        }
        
        return "Unavailable";
    }

    public string GetEmail()
    {
        var identity = GetUserIdentityAsClaims();
        if (identity != null)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown Email";   
        }
        
        return "Unavailable";
    }

    public string GetFirstName()
    {
        var identity = GetUserIdentityAsClaims();
        if (identity != null)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "Unknown First Name";   
        }
        
        return "Unavailable";
    }

    public string GetLastName()
    {
        var identity = GetUserIdentityAsClaims();
        if (identity != null)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "Unknown Last Name";   
        }
        
        return "Unavailable";
    }
}