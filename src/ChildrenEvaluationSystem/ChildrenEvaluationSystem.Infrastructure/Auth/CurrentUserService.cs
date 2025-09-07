using System.Security.Claims;
using ChildrenEvaluationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ChildrenEvaluationSystem.Infrastructure.Auth;

public class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
{
    public string UserId
    {
        get
        {
            var principal = http.HttpContext?.User;
            if (principal == null || !principal.Identity?.IsAuthenticated == true) return string.Empty;
            
            return principal.FindFirstValue("oid")
                   ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? principal.FindFirstValue("sub")
                   ?? string.Empty;
        }
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
}