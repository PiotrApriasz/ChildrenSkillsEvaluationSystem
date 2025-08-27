namespace ChildrenEvaluationSystem.Application.Interfaces;

public interface IAuthenticationService
{
    Task LoginAsync();
    Task LogoutAsync();
    bool IsAuthenticatedAsync();
    string GetUsername();
    string GetEmail();
    string GetFirstName();
    string GetLastName();
}