namespace ChildrenEvaluationSystem.Application.Interfaces;

public interface IAuthenticationService
{
    Task LoginAsync(string? returnUrl = "/");
    Task LogoutAsync(string? returnUrl = "/");
    bool IsAuthenticatedAsync();
    string GetUsername();
    string GetEmail();
    string GetFirstName();
    string GetLastName();
}