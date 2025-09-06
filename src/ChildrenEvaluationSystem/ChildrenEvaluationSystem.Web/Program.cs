using ChildrenEvaluationSystem.Web.SharedComponents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using MudBlazor.Services;
using AuthenticationService = ChildrenEvaluationSystem.Infrastructure.Auth.AuthenticationService;
using IAuthenticationService = ChildrenEvaluationSystem.Application.Interfaces.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddControllersWithViews();

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureEntra"));

builder.Services.PostConfigure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
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
        
        var postLogout = builder.Configuration["AzureEntra:SignedOutRedirectUri"] ?? "/";
        ctx.ProtocolMessage.PostLogoutRedirectUri =
            $"{ctx.Request.Scheme}://{ctx.Request.Host}{postLogout}";
    };
    
    options.Scope.Add("openid");
    options.Scope.Add("profile");
});

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.Configure<CookieAuthenticationOptions>(
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

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();