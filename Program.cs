using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;
using NUBULUS.AccountsAppsPortalBackEnd.Application;
using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Users.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

const string CorsPolicy = "AllowFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("X-User-Auth")
    );
});


builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

app.UseRouting();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();


// Auth Endpoints
app.MapSignInEndpoint().MapSignOutEndpoint().MapIsValidSessionEndpoint();



app.MapGet("/api/v1/auth/sign-out", async (HttpContext context) =>
{
    var returnUrl = context.Request.Query["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        returnUrl = "http://localhost:5173/";
    }

    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    var isAuth = context.User?.Identity?.IsAuthenticated ?? false;
    if (!isAuth)
    {
        context.Response.Redirect(returnUrl);
        return;
    }

    var props = new AuthenticationProperties { RedirectUri = returnUrl };
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
}).AllowAnonymous();


app.Run();
