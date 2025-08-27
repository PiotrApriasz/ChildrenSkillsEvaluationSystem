using ChildrenEvaluationSystem.Web.SharedComponents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MudBlazor.Services;
using AuthenticationService = ChildrenEvaluationSystem.Infrastructure.Auth.AuthenticationService;
using IAuthenticationService = ChildrenEvaluationSystem.Application.Interfaces.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();


// Configure Microsoft Entra External ID authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "ChildrenEvaluationSystem.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
    options.LoginPath = "/";
    options.LogoutPath = "/signout";
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // Basic OIDC settings
    options.Authority = $"{builder.Configuration["AzureEntra:Instance"]}/{builder.Configuration["AzureEntra:Domain"]}/v2.0/";
    options.ClientId = builder.Configuration["AzureEntra:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Schemas:MicrosoftOidc:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    
    // Callback paths
    options.CallbackPath = builder.Configuration["AzureEntra:CallbackPath"];
    options.SignedOutCallbackPath = builder.Configuration["AzureEntra:SignedOutCallbackPath"];
    
    // Scopes
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    
    // Token validation
    options.TokenValidationParameters.ValidateIssuer = true;
    options.TokenValidationParameters.ValidIssuer = $"{builder.Configuration["AzureEntra:Instance"]}/{builder.Configuration["AzureEntra:TenantId"]}/v2.0/";
    options.TokenValidationParameters.ValidateAudience = true;
    options.TokenValidationParameters.ValidAudience = builder.Configuration["AzureEntra:ClientId"];
    options.TokenValidationParameters.ValidateLifetime = true;
    options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
    
    // Additional settings
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.UseTokenLifetime = false;
    
    // Handle sign-in/sign-out events
    options.Events = new OpenIdConnectEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/?error=authentication_failed");
            return Task.CompletedTask;
        },
        OnAccessDenied = context =>
        {
            context.HandleResponse();
            context.Response.Redirect("/?error=access_denied");
            return Task.CompletedTask;
        },
        OnSignedOutCallbackRedirect = context =>
        {
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
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

app.UseAntiforgery();

app.MapGet("/challenge-microsoft", async (HttpContext context) =>
{
    await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, 
        new AuthenticationProperties { RedirectUri = "/" });
});

// Add logout endpoint
app.MapGet("/signout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();