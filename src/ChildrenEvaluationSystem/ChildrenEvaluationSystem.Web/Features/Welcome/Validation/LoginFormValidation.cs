using System.Text.RegularExpressions;

namespace ChildrenEvaluationSystem.Web.Features.Welcome.Validation;

public static class LoginFormValidation
{
    public static string? ValidateEmail(string v)
    {
        if (string.IsNullOrWhiteSpace(v)) return "Email jest wymagany";
        return Regex.IsMatch(v, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") ? null : "Niepoprawny format adresu email";
    }
    
    public static string? ValidatePassword(string v)
    {
        return string.IsNullOrWhiteSpace(v) ? "Hasło jest wymagane" : null;
    }
}