# Guía para Agentes de IA - NUBULUS AccountsAppsPortal Backend

## Arquitectura y Estructura del Proyecto

Este proyecto es una API backend construida con ASP.NET Core 8 utilizando Minimal APIs, siguiendo una arquitectura limpia (Clean Architecture) con las siguientes capas:

- **Domain** (`domain/`): Entidades, Value Objects y abstracciones core

  - **Entidades principales**: `AccountEntity`, `User`
  - **Value Objects**: `AccountKey`, `EmailAddress`, `Status`
  - **Abstracciones**: `IAccountsRepository`, `IUnitOfWork`, `IDomainEvent`
  - **Comandos de dominio**: `CreateAccount` (con validadores internos)

- **API** (`api/`): Capa de presentación con Minimal APIs

  - **Features**: Organizadas por módulos funcionales (`Account/`)
  - Cada feature contiene: endpoints, servicios y requests
  - `DI.cs` a nivel global y por módulo para registro de servicios
  - **Common**: Tipos compartidos como `IGenericResponse<T>`, `ResultType`

- **Infrastructure** (`infraestructure/pgsql/`): Implementaciones técnicas
  - **PostgreSQL** como base de datos principal con Entity Framework Core
  - `PostgreDBContext` para acceso a datos
  - **Models**: Modelos de persistencia (diferentes a entidades de dominio)
  - **Repositories**: Implementaciones de repositorios del dominio
  - **UnitOfWork**: Patrón Unit of Work para transacciones

## Patrones y Estilo de Programación

### Estructura de Request/Response

- Los **requests** son clases con inicializadores de propiedades (`init`)
- Tienen constructor por defecto para deserialización
- Constructor privado con validación + Factory method `Create` (opcional)
- Método `Validate()` público que lanza excepciones:

```csharp
public class CreateAccountRequest
{
    public string Name { get; init; }
    public string Email { get; init; }

    // Constructor por defecto para deserialización
    public CreateAccountRequest()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    // Constructor privado con validación
    private CreateAccountRequest(string name, string email)
    {
        Name = name;
        Email = email;
        Validate();
    }

    // Método de validación público
    public CreateAccountRequest Validate()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 100)
            throw new ArgumentException("Name must be between 1 and 100 characters.");

        // Más validaciones...
        return this;
    }

    // Factory method opcional
    public static CreateAccountRequest Create(string name, string email)
    {
        return new CreateAccountRequest(name, email);
    }
}
```

- Las **responses** implementan `IGenericResponse<T>` como clases internas del servicio
- Factory methods estáticos para cada resultado: `Success`, `DataExists`, `Error`, `ValidationError`

```csharp
internal class CreateAccountResponse : IGenericResponse<string>
{
    public ResultType ResultType { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    public string? Data { get; set; }

    private CreateAccountResponse(ResultType resultType, string? message,
                                  Dictionary<string, string[]>? validationErrors, string? data)
    {
        ResultType = resultType;
        Message = message;
        ValidationErrors = validationErrors;
        Data = data;
    }

    public static CreateAccountResponse Success(string data) =>
        new(ResultType.Ok, null, null, data);

    public static CreateAccountResponse DataExists(string message) =>
        new(ResultType.Conflict, message, null, null);
}
```

### Manejo de Errores y Resultados

- Usar enum `ResultType` para estados de operación:
  - `Ok`: Operación exitosa
  - `Conflict`: Conflicto con datos existentes
  - `Problems`: Errores de validación
  - `NotFound`: Recurso no encontrado
  - `Error`: Errores inesperados
  - `None`: Estado inicial
- **No usar excepciones para flujo de control normal**
- Las validaciones en Requests/Commands lanzan excepciones capturadas en servicios
- Los servicios devuelven responses tipadas con estado
- Validaciones en bloques `try-catch` convertidas a `ValidationError`

## Patrones y Convenciones

### Estructura de Módulos y Casos de Uso

Cada módulo (feature) sigue esta estructura:

```
api/Features/
    ├── Common/
    │   ├── IGenericResponse.cs       # Interface genérica para responses
    │   └── ResultType.cs             # Enum con tipos de resultado
    ├── DI.cs                          # Registro global de features
    └── {FeatureName}/                 # Ej: Account/
        ├── DI.cs                      # Registro del feature específico
        ├── {Request}.cs               # Ej: CreateAccountRequest.cs
        └── {UseCase}/                 # Ej: CreateAccount/
            ├── {UseCase}EndPoint.cs   # Minimal API endpoint
            └── {UseCase}Service.cs    # Lógica del caso de uso
```

**Ejemplo concreto**:

```
api/Features/
    ├── Common/
    ├── DI.cs
    └── Account/
        ├── DI.cs
        ├── CreateAccountRequest.cs
        └── CreateAccount/
            ├── CreateAccountEndPoint.cs
            └── CreateAccountService.cs
```

### Endpoints (Minimal APIs)

- Rutas base: `/api/v1/{resource}` (plural)
- Un archivo por endpoint siguiendo patrón `{Action}{Entity}EndPoint.cs`
- Método de extensión `Map{Action}{Entity}EndPoint` que retorna `WebApplication`
- Configuración con `.WithName()` y `.WithTags()`
- Sin autenticación por defecto (agregar `.RequireAuthorization()` si es necesario)

```csharp
public static class CreateAccountEndPoint
{
    public static WebApplication MapCreateAccountEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts", async (
            CreateAccountRequest request,
            CreateAccountService service,
            CancellationToken cancellationToken) =>
        {
            var response = await service.ExecuteAsync(request, cancellationToken);
            return response.ResultType switch
            {
                ResultType.Ok => Results.Created($"/api/v1/accounts/{response.Data}", null),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateAccount")
        .WithTags("Accounts");

        return app;
    }
}
```

### Servicios

- Una clase por caso de uso: `{Action}{Entity}Service`
- **Response interna**: Clase `{Action}{Entity}Response` dentro del servicio implementando `IGenericResponse<T>`
- Método principal: `ExecuteAsync(Request request, CancellationToken cancellationToken)`
- Constructor con inyección de dependencias (repositorios, etc.)

```csharp
public class CreateAccountService
{
    // Response interna
    internal class CreateAccountResponse : IGenericResponse<string>
    {
        public ResultType ResultType { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
        public string? Data { get; set; }

        private CreateAccountResponse(ResultType resultType, string? message,
                                      Dictionary<string, string[]>? validationErrors,
                                      string? data) { /* ... */ }

        public static CreateAccountResponse Success(string data) => /* ... */
        public static CreateAccountResponse DataExists(string message) => /* ... */
        public static CreateAccountResponse Error(string message) => /* ... */
        public static CreateAccountResponse ValidationError(
            Dictionary<string, string[]> errors) => /* ... */
    }

    private readonly IAccountsRepository _accountsRepository;

    public CreateAccountService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<IGenericResponse<string>> ExecuteAsync(
        CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Validar request
        try { request.Validate(); }
        catch (Exception ex)
        {
            return CreateAccountResponse.ValidationError(
                new Dictionary<string, string[]> {
                    { "Request", new[] { ex.Message } }
                });
        }

        // 2. Verificar existencia
        var exists = await _accountsRepository.AccountInfoExistsAsync(/*...*/);
        if (exists)
            return CreateAccountResponse.DataExists("Account already exists.");

        // 3. Ejecutar operación
        try
        {
            var command = new CreateAccount(/*...*/);
            await _accountsRepository.CreateAccountAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            return CreateAccountResponse.Error($"Error: {ex.Message}");
        }

        // 4. Retornar éxito
        return CreateAccountResponse.Success(data);
    }
}
```

### Repositorios

- Interfaz en `domain/Abstractions/I{Entity}Repository.cs`
- Implementación en `infraestructure/pgsql/Repositories/{Entity}Repository.cs`
- **No separar en Commands/Queries** - Un solo repositorio por entidad
- Métodos asíncronos con `CancellationToken` opcional por defecto

```csharp
// domain/Abstractions/IAccountsRepository.cs
public interface IAccountsRepository
{
    Task<AccountEntity> GetAccountByKeyAsync(string accountKey,
                                             CancellationToken cancellationToken = default);
    Task<IQueryable<AccountEntity>> GetAllAccountsAsync(
                                             CancellationToken cancellationToken = default);
    Task CreateAccountAsync(CreateAccount command,
                           CancellationToken cancellationToken = default);
    Task UpdateAccountAsync(AccountEntity command,
                           CancellationToken cancellationToken = default);
    Task<bool> AccountInfoExistsAsync(string name, string email, string phone,
                                      string numberId,
                                      CancellationToken cancellationToken = default);
}
```

### Registro de Dependencias (DI)

- **Archivo global**: `api/Features/DI.cs` registra todas las features y mapea endpoints
- **Archivo por feature**: `api/Features/{Feature}/DI.cs` registra servicios y repositorios del feature

```csharp
// api/Features/DI.cs
public static class DI
{
    public static IServiceCollection AddApplicationFeature(this IServiceCollection services)
    {
        services.AddAccountFeature();
        // services.AddOtherFeature();
        return services;
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapAccountEndpoints();
        // app.MapOtherEndpoints();
        return app;
    }
}

// api/Features/Account/DI.cs
public static class DI
{
    public static IServiceCollection AddAccountFeature(this IServiceCollection services)
    {
        services.AddScoped<CreateAccountService>();
        services.AddScoped<IAccountsRepository, AccountRepository>();
        return services;
    }

    public static WebApplication MapAccountEndpoints(this WebApplication app)
    {
        app.MapCreateAccountEndPoint();
        // app.MapOtherEndPoint();
        return app;
    }
}
```

## Capa de Dominio (Domain)

### Entidades de Dominio

- Clases simples con propiedades públicas
- Ubicadas en `domain/Entities/{EntityName}/`
- Usan Value Objects para propiedades complejas

```csharp
// domain/Entities/Account/AccountEntity.cs
public class AccountEntity
{
    public int Id { get; set; }
    public AccountKey AccountKey { get; set; } = default!;
    public string Name { get; set; } = default!;
    public EmailAddress Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public Status Status { get; set; } = Status.Active;
}
```

### Comandos de Dominio

- Clases con lógica de validación interna
- Constructor público que ejecuta validación
- Validador interno `sealed class` con lógica de validación

```csharp
// domain/Entities/Account/CreateAccount.cs
public class CreateAccount
{
    public string Key { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    // ... más propiedades

    public CreateAccount(string key, string name, EmailAddress email, /* ... */)
    {
        Key = key;
        Name = name;
        Email = email;
        // ... asignar propiedades

        // Ejecutar validación al instanciar
        CreateAccountValidator validator = new CreateAccountValidator(this);
    }
}

internal sealed class CreateAccountValidator
{
    public CreateAccountValidator(CreateAccount command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Key))
            throw new ArgumentException("Account key is required.");

        if (command.Key.Length > 36)
            throw new ArgumentException("Account key must not exceed 36 characters.");

        // ... más validaciones
    }
}
```

### Value Objects

- Encapsulan validación y lógica de dominio
- Propiedades inmutables con `private set`
- Validación en constructor o método `Validate()`

```csharp
// domain/ValueObjects/AccountKey.cs
public class AccountKey
{
    public string Value { get; private set; } = default!;

    public AccountKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Account key cannot be null or empty.");
        if (value.Length > 36)
            throw new ArgumentException("Account key must not exceed 36 characters.");

        Value = value;
    }
}

// domain/ValueObjects/EmailAddress.cs
public class EmailAddress
{
    public string Value { get; set; }

    public EmailAddress(string address)
    {
        Value = address;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value) || !Value.Contains("@"))
            throw new ArgumentException("Invalid email address.");

        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(Value, pattern, RegexOptions.IgnoreCase))
            throw new ArgumentException("Email address format is invalid.");
    }
}

// domain/ValueObjects/Status.cs (Enum pattern)
public class Status
{
    public string Value { get; private set; }

    private Status(string value) { Value = value; }

    public static Status Active => new Status("Active");
    public static Status Inactive => new Status("Inactive");
}
```

### Abstracciones

- Interfaces de repositorios en `domain/Abstractions/`
- `IUnitOfWork` para transacciones
- `IDomainEvent` para eventos de dominio (si se usa)

```csharp
// domain/Abstractions/IAccountsRepository.cs
public interface IAccountsRepository
{
    Task<AccountEntity> GetAccountByKeyAsync(string accountKey,
                                             CancellationToken cancellationToken = default);
    Task CreateAccountAsync(CreateAccount command,
                           CancellationToken cancellationToken = default);
    Task<bool> AccountInfoExistsAsync(string name, string email,
                                      CancellationToken cancellationToken = default);
}
```

## Capa de Infraestructura (Infrastructure)

### PostgreSQL con Entity Framework Core

- DbContext en `infraestructure/pgsql/PostgreDBContext.cs`
- Modelos de persistencia en `infraestructure/pgsql/Models/`
- **Importante**: Los modelos de persistencia son DIFERENTES a las entidades de dominio

### Modelos de Persistencia

- POCOs con propiedades públicas
- Configuración Fluent API mediante `IEntityTypeConfiguration<T>`
- Configuración inline en la misma clase

```csharp
// infraestructure/pgsql/Models/Account.cs
public class Account
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string NumberId { get; set; } = string.Empty;
    public string Status { get; set; } = "A";

    public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();
}

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(a => a.Key).HasColumnName("key").IsRequired().HasMaxLength(36);
        builder.Property(a => a.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        // ... más configuraciones
    }
}
```

### DbContext

- Registra configuraciones con `ApplyConfiguration`
- DbSets para cada entidad

```csharp
// infraestructure/pgsql/PostgreDBContext.cs
public class PostgreDBContext : DbContext
{
    public PostgreDBContext(DbContextOptions<PostgreDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountUserConfiguration());
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AccountUser> AccountUsers { get; set; }
}
```

### Repositorios

- Implementan interfaces de `domain/Abstractions/`
- **Mapeo manual** de modelos de persistencia a entidades de dominio
- Trabajan directamente con el DbContext

```csharp
// infraestructure/pgsql/Repositories/AccountRepository.cs
public class AccountRepository : IAccountsRepository
{
    private readonly PostgreDBContext _dbContext;

    public AccountRepository(PostgreDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAccountAsync(CreateAccount command,
                                        CancellationToken cancellationToken = default)
    {
        // Crear modelo de persistencia desde comando de dominio
        var account = new Account
        {
            Key = command.Key,
            Name = command.Name,
            Email = command.Email.Value,  // Extraer valor del Value Object
            Phone = command.Phone,
            Status = "A"
        };

        await _dbContext.Accounts.AddAsync(account, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AccountInfoExistsAsync(string name, string email,
                                                   CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts.AnyAsync(a =>
            a.Name == name || a.Email == email, cancellationToken);
    }
}
```

## Configuración y Dependencias

### Program.cs

- Configuración mínima y limpia
- Registro de DbContext con PostgreSQL
- Inyección de features mediante método de extensión
- Mapeo de endpoints mediante método de extensión

```csharp
using Microsoft.EntityFrameworkCore;
using Nubulus.Backend.Api.Features;
using Nubulus.Backend.Infraestructure.Pgsql;

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
```

### Base de Datos PostgreSQL

Configuración en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQLConnection": "Host=localhost;Port=5432;Database=testdb;Username=admin;Password=admin123"
  }
}
```

### Migraciones Entity Framework Core

- Migraciones ubicadas en `api/Migrations/` (no en el proyecto de infraestructura)
- Assembly de migraciones configurado: `"nubulus.backend.api"`
- **Crear migración**: `dotnet ef migrations add NombreMigracion --project api`
- **Aplicar migración**: `dotnet ef database update --project api`
- **Rollback**: `dotnet ef database update MigracionAnterior --project api`

### Docker Compose

Configuración en `.docker/docker-compose.yml`:

```yaml
services:
  postgres:
    image: postgres:16-alpine
    container_name: desc_postgres
    restart: always
    environment:
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: admin123
      POSTGRES_DB: testdb
    ports:
      - "5432:5432"
    volumes:
      - pgsql_data:/var/lib/postgresql/data
    networks:
      - backend

  webapi:
    build:
      dockerfile: ../Dockerfile
    container_name: nubulus_api
    restart: always
    ports:
      - "8080:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__Postgres: "Host=postgres;Port=5432;Database=testdb;Username=admin;Password=admin123"
    depends_on:
      - postgres
    networks:
      - backend
```

### Autenticación (Azure AD / Microsoft Identity)

Configuración en `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "localhost:5016",
    "TenantId": "tu-tenant-id",
    "ClientId": "tu-client-id",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

**Nota**: Actualmente configurado pero no implementado en los endpoints. Agregar `.RequireAuthorization()` cuando se necesite.

## Flujos de Trabajo Comunes

### Crear una Nueva Feature

1. **Crear estructura de carpetas**:

   ```
   api/Features/{FeatureName}/
       ├── DI.cs
       ├── {Request}.cs
       └── {UseCase}/
           ├── {UseCase}EndPoint.cs
           └── {UseCase}Service.cs
   ```

2. **Definir Request** en `api/Features/{Feature}/{Request}.cs`:

   - Constructor por defecto (deserialización)
   - Constructor privado con validación
   - Método `Validate()` público
   - Factory method `Create()` (opcional)

3. **Crear Servicio** en `api/Features/{Feature}/{UseCase}/{UseCase}Service.cs`:

   - Response interna implementando `IGenericResponse<T>`
   - Factory methods para cada resultado
   - Método `ExecuteAsync()` con lógica de negocio

4. **Crear Endpoint** en `api/Features/{Feature}/{UseCase}/{UseCase}EndPoint.cs`:

   - Método de extensión `Map{UseCase}EndPoint`
   - Configurar ruta, verbo HTTP, nombre y tags
   - Pattern matching en `ResultType`

5. **Registrar en DI** (`api/Features/{Feature}/DI.cs`):

   - Registrar servicios y repositorios
   - Mapear endpoints

6. **Actualizar DI global** (`api/Features/DI.cs`):
   - Agregar `services.Add{Feature}Feature()`
   - Agregar `app.Map{Feature}Endpoints()`

### Crear una Nueva Entidad de Dominio

1. **Entidad** en `domain/Entities/{Entity}/{Entity}Entity.cs`
2. **Value Objects** necesarios en `domain/ValueObjects/`
3. **Comandos** en `domain/Entities/{Entity}/Create{Entity}.cs` con validador interno
4. **Repositorio** (interfaz en `domain/Abstractions/`)
5. **Modelo de persistencia** en `infraestructure/pgsql/Models/{Entity}.cs` con configuración
6. **Implementación repositorio** en `infraestructure/pgsql/Repositories/{Entity}Repository.cs`
7. **Migración**: `dotnet ef migrations add Add{Entity} --project api`

## Puntos de Integración

- **Frontend**: Aplicación Vue.js en puerto 5173 (pendiente de integración)
- **PostgreSQL**: Base de datos principal en puerto 5432
- **Swagger UI**: Documentación de API en `/swagger` (solo Development)
- **Microsoft Identity**: Configurado para autenticación OpenID Connect (no activo actualmente)

## Consideraciones para Desarrollo

- Usar **Value Objects** para encapsular lógica de validación (ej: `EmailAddress`, `AccountKey`)
- Mantener **endpoints minimalistas** - toda la lógica en servicios
- **No separar repositorios** en Commands/Queries - un solo repositorio por entidad
- Las **responses van dentro del servicio** como clase interna
- Modelos de **persistencia != entidades de dominio** - hacer mapeo manual
- **Validación dual**: Request (formato) y Comando (reglas de negocio)
- **Excepciones** solo para validación, no para flujo de control
- Los servicios devuelven **estados tipados** mediante `IGenericResponse<T>`
- Usar **CancellationToken** en todos los métodos asíncronos
- **Migraciones** en el proyecto API, no en Infrastructure
- Configuración de EF Core con **Fluent API** inline en modelos de persistencia
