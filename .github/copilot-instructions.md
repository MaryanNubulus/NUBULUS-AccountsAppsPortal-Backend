# Guía para Agentes de IA - NUBULUS AccountsAppsPortal Backend

## Arquitectura y Estructura del Proyecto

Este proyecto es una API backend construida con ASP.NET Core 8 utilizando Minimal APIs, siguiendo una arquitectura limpia (Clean Architecture) con las siguientes capas:

- **Domain**: Entidades y abstracciones core (`Domain/`)

  - Interfaces base: `ICommand`, `IQuery`, `IGenericResponse`
  - Entidades principales: `Account`, `App`, `Employee`, `User`

- **Application**: Lógica de negocio y casos de uso (`Application/`)

  - Features organizadas por módulos: `Auth`, `Apps`, `Employees`
  - Cada módulo tiene su propio `DI.cs` para registro de servicios
  - DTOs y modelos específicos en `Common/Models`
  - Casos de uso organizados por carpetas (ej: `CreateApp/`)

- **Infrastructure**: Implementaciones técnicas (`Infrastructure/`)
  - MongoDB como base de datos principal
  - `MongoDBClient` como implementación de `INoSQLClient`

## Patrones y Estilo de Programación

### Estructura de Request/Response

- Usar `record` para DTOs inmutables (ver `CreateAppRequest`)
- Validación encapsulada en los propios modelos:

```csharp
public record CreateAppRequest
{
    // Constructor privado para validación
    private CreateAppRequest(string key, string name)
    {
        Key = key;
        Name = name;
        Validate();
    }

    // Factory method público
    public static CreateAppRequest Create(string key, string name)
    {
        return new CreateAppRequest(key, name);
    }
}
```

### Manejo de Errores y Resultados

- Usar `ResultType` enum para estados de operación:
  - `Ok`: Operación exitosa
  - `Conflict`: Conflicto con datos existentes
  - `Problems`: Errores de validación
  - `Error`: Errores inesperados
- Mantener mensajes de error en el servicio
- Usar diccionario para errores de validación

## Patrones y Convenciones

### Estructura de Módulos y Casos de Uso

Cada módulo sigue esta estructura:

```
Application/Common/Requests/
                    └── CreateEntityRequest.cs
Application/Features/ModuleName/
                            ├── README.md
                            ├── DI.cs
                            ├── Common/
                            │   └── EntityMappers.cs
                            └── CreateEntity/
                                ├── CreateEntityEndpoint.cs
                                ├── CreateEntityService.cs
                                └── ICreateEntityService.cs
```

### Endpoints (Minimal APIs)

- Rutas base: `/api/v1/{module}`
- Un archivo por endpoint
- Usar extensión de `WebApplication`
- Autenticación vía OpenID Connect (Microsoft)
- CORS configurado para `http://localhost:5173`
- Patrón de respuesta consistente:

```csharp
public static WebApplication MapEndpoint(this WebApplication app)
{
    app.MapPost("/api/v1/resource", async ([FromServices] IService service,
                                         [FromBody] Request request) =>
    {
        await service.ExecuteAsync(request);
        return service.ResultType switch
        {
            ResultType.Ok => Results.Created(),
            ResultType.Conflict => Results.Conflict(new { service.Message }),
            ResultType.Problems => Results.ValidationProblem(service.ValidationErrors),
            _ => Results.Problem(service.Message)
        };
    }).RequireAuthorization();

    return app;
}
```

### Servicios y Repositorios

- Interfaces claras con responsabilidad única
- Separación CQRS:
  - `I{Entity}CommandsRepository`
  - `I{Entity}QueriesRepository`
- Estado de resultado en el servicio (no excepciones para flujo normal)
- Validación antes de operaciones de BD
- Verificación de existencia/unicidad
- Métodos asíncronos para IO

## Configuración y Dependencias

### Base de Datos

```json
{
  "ConnectionStrings": {
    "MongoDB": "tu_connection_string"
  }
}
```

### Autenticación

Requiere configuración en `appsettings.json`:

```json
{
  "AzureAd": {
    // Configuración de Microsoft Identity
  }
}
```

## Flujos de Trabajo Comunes

### Autenticación

1. Frontend redirige a `/api/v1/auth/sign-in`
2. Proceso OAuth con Microsoft
3. Redirige a `/api/v1/auth/success`
4. Crea empleado automáticamente si no existe

### Gestión de Apps

- Crear app: POST `/api/v1/apps`
- Listar apps: GET `/api/v1/apps`
- Pausar/reanudar: POST `/api/v1/apps/{id}/pause|resume`

## Puntos de Integración

- **Frontend**: Aplicación Vue.js en puerto 5173
- **Microsoft Identity**: Para autenticación OpenID Connect
- **MongoDB**: Para persistencia de datos

## Consideraciones para Desarrollo

- Usar Value Objects para encapsular lógica de validación (ver `Email.cs`)
- Mantener endpoints minimalistas - lógica compleja en servicios
- Seguir el patrón de separación comandos/consultas existente
- Documentar nuevos módulos siguiendo el formato de los READMEs existentes
