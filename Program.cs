using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using NUBULUS.AccountsAppsPortalBackEnd.Infraestructure;
using NUBULUS.AccountsAppsPortalBackEnd.Application;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

BsonSerializer.RegisterSerializer(
    typeof(Guid),
    new GuidSerializer(GuidRepresentation.Standard)
);

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


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NUBULUS Accounts Apps Portal API",
        Version = "v1",
        Description = "API para la gestión de cuentas y aplicaciones de NUBULUS"
    });

    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
        Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
        {
            AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["AzureAd:AuthorizationUrl"]),
                TokenUrl = new Uri(builder.Configuration["AzureAd:TokenUrl"])
            }
        }
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure();
builder.Services.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        options.OAuthScopes("openid", "profile");
        options.OAuthUsePkce();
    });
}

app.UseRouting();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();


app.MapApplicationEndpoints();

app.Run();
