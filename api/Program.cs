using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Api.Features;
using Nubulus.Backend.Infraestructure.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgreDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQLConnection"),
        b => b.MigrationsAssembly("nubulus.backend.api")
    ));

builder.Services.AddApplicationFeature();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapApplicationEndpoints();

app.Run();