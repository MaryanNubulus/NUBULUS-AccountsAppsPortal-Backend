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

- **Infrastructure**: Implementaciones técnicas (`Infrastructure/`)
  - MongoDB como base de datos principal
  - `MongoDBClient` como implementación de `INoSQLClient`

## Patrones y Convenciones

### Estructura de Módulos

Cada módulo (Auth, Apps, Employees) sigue una estructura consistente:

- `README.md` con documentación específica
- Endpoints en archivos separados (ej: `SignInEndpoint.cs`)
- Carpetas por caso de uso (ej: `CreateApp/`, `GetApps/`)

### Endpoints

- Rutas base: `/api/v1/{module}`
- Implementados como Minimal APIs
- Autenticación vía OpenID Connect (Microsoft)
- CORS configurado para `http://localhost:5173`

### Repositorios

- Interfaces separadas para comandos y consultas:
  - `I{Entity}CommandsRepository`
  - `I{Entity}QueriesRepository`
- Implementaciones en `Application/Features/Repositories`

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
