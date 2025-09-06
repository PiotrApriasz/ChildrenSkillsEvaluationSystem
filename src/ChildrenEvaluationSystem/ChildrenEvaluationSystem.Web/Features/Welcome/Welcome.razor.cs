using System.Security.Claims;
using ChildrenEvaluationSystem.Web.Features.Welcome.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ChildrenEvaluationSystem.Web.Features.Welcome;

public partial class Welcome : ComponentBase
{
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = null!;

    private string _email = string.Empty;
    private string _password = string.Empty;
    
    private string? EmailError { get; set; }
    private string? PasswordError { get; set; }
    
    private static string MicrosoftIcon => "<svg viewBox='0 0 23 23' xmlns='http://www.w3.org/2000/svg'><path fill='#f35325' d='M1 1h10v10H1z'/><path fill='#81bc06' d='M12 1h10v10H12z'/><path fill='#05a6f0' d='M1 12h10v10H1z'/><path fill='#ffba08' d='M12 12h10v10H12z'/></svg>";
    private static string GoogleIcon => "<svg viewBox='0 0 24 24' xmlns='http://www.w3.org/2000/svg'><path fill='#4285f4' d='M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z'/><path fill='#34a853' d='M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z'/><path fill='#fbbc05' d='M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z'/><path fill='#ea4335' d='M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z'/></svg>";
    
    private string Email {
        get => _email;
        set {
            _email = value;
            EmailError = LoginFormValidation.ValidateEmail(_email);
        }
    }

    private string Password {
        get => _password;
        set {
            _password = value;
            PasswordError = LoginFormValidation.ValidatePassword(_password);
        }
    }
    
    private bool IsValid =>
        string.IsNullOrEmpty(EmailError) &&
        string.IsNullOrEmpty(PasswordError) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password);
    
    protected override void OnInitialized()
    {
        var uri = new Uri(Navigation.Uri);
        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

        switch (queryParams["error"])
        {
            case "authentication_failed":
                Snackbar.Add("Błąd podczas logowania. Spróbuj ponownie.", Severity.Error);
                break;
            case "access_denied":
                Snackbar.Add("Dostęp został odrzucony.", Severity.Error);
                break;
        }
    }
    

    private async Task HandleLogin()
    {
        EmailError = LoginFormValidation.ValidateEmail(Email);
        PasswordError = LoginFormValidation.ValidatePassword(Password);
        if (!IsValid) return;
        
        await Task.CompletedTask;
    }

    private void HandleRegister()
    {
       
    }
    
    private void HandleMicrosoftLogin()
    {
        try
        {
            Navigation.NavigateTo("/challenge-microsoft", forceLoad: true);
        }
        catch (Exception ex)
        {
            Snackbar.Add("Błąd podczas przekierowania do Microsoft. Spróbuj ponownie.", Severity.Error);
        }
    }

    private async Task HandleGoogleLogin()
    {
        await Task.CompletedTask;
    }
}