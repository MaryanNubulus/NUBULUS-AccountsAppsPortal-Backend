# Feature d'Apps - Guia Pràctica

## Resum

Gestió d'aplicacions amb les següents funcionalitats:

- ✅ Crear aplicacions amb validació completa
- ✅ Llistar aplicacions amb paginació i cerca
- ✅ Actualitzar informació d'aplicacions (Key i Name)
- ✅ Pausar/Reactivar aplicacions
- ✅ Validació en múltiples capes (Request, Domain, Repository)
- ✅ Auditoria completa de totes les operacions
- ✅ Control de duplicats per Key

**Tecnologies**: ASP.NET Core 8 + PostgreSQL + Entity Framework Core + Minimal APIs

---

## Índex

1. [Endpoints](#endpoints)
2. [Requests i Validacions](#requests-i-validacions)
3. [Exemples d'Ús](#exemples-dús)
4. [Errors i Respostes](#errors-i-respostes)
5. [Validació Multi-Capa](#validació-multi-capa)
6. [Notes Importants](#notes-importants)

---

## Endpoints

Tots els endpoints requereixen **autenticació** (Bearer Token).

### Resum d'Endpoints

| Mètode | Ruta                       | Descripció               |
| ------ | -------------------------- | ------------------------ |
| POST   | `/api/v1/apps`             | Crear aplicació          |
| GET    | `/api/v1/apps`             | Llistar aplicacions      |
| GET    | `/api/v1/apps/{id}`        | Obtenir aplicació per ID |
| PUT    | `/api/v1/apps/{id}`        | Actualitzar aplicació    |
| PATCH  | `/api/v1/apps/{id}/pause`  | Pausar aplicació         |
| PATCH  | `/api/v1/apps/{id}/resume` | Reactivar aplicació      |

---

## Requests i Validacions

### 1. CreateAppRequest

**Endpoint**: `POST /api/v1/apps`

```csharp
public class CreateAppRequest
{
    public string Key { get; init; }   // Clau única
    public string Name { get; init; }  // Nom de l'aplicació
}
```

**Validacions**:

- `Key`: entre 3 i 100 caràcters, només lletres, números, guions, sense espais i caracters especials.
- `Name`: entre 3 i 100 caràcters.

```csharp
public Dictionary<string, string[]> Validate()
{
    var errors = new Dictionary<string, string[]>();

    // Key: entre 3 i 100 caràcters, format vàlid
    if (string.IsNullOrWhiteSpace(Key) || Key.Length < 3 || Key.Length > 100)
        errors["Key"] = new[] { "Key must be between 3 and 100 characters." };
    else
    {
        // Solo letras, números, guiones, sin espacios ni caracteres especiales
        var keyPattern = @"^[a-zA-Z0-9\-]+$";
        if (!Regex.IsMatch(Key, keyPattern))
            errors["Key"] = new[] { "Key can only contain letters, numbers, and hyphens." };
    }

    // Name: entre 3 i 100 caràcters
    if (string.IsNullOrWhiteSpace(Name) || Name.Length < 3 || Name.Length > 100)
        errors["Name"] = new[] { "Name must be between 3 and 100 characters." };

    return errors;
}
```

**Exemple de Request**:

```json
{
  "key": "my-app-key",
  "name": "Aplicació Principal"
}
```

### 2. GetAppsRequest

**Endpoint**: `GET /api/v1/apps?searchTerm=my&pageNumber=1&pageSize=20`

```csharp
public class GetAppsRequest
{
    public string? SearchTerm { get; set; } // Cerca opcional
    public int? PageNumber { get; set; }    // Default: 1
    public int? PageSize { get; set; }      // Default: 10
}
```

**Response**:

```json
{
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 20,
  "items": [
    {
      "id": 1,
      "key": "my-app-key",
      "name": "Aplicació Principal",
      "status": "A"
    }
  ]
}
```

### 3. GetAppRequest

**Endpoint**: `GET /api/v1/apps/{id}`

```csharp
public class GetAppRequest
{
    public int Id { get; init; }  // ID de l'aplicació
}
```

**Response**:

```json
{
  "id": 123,
  "key": "my-app-key",
  "name": "Aplicació Principal",
  "status": "A"
}
```

### 4. UpdateAppRequest

**Endpoint**: `PUT /api/v1/apps/{id}`

```csharp
public class UpdateAppRequest
{
    public string Name { get; init; }
}
```

**Validacions**:

- `Name`: entre 3 i 100 caràcters.

**⚠️ Nota Important**: La `Key` **NO es pot actualitzar**. És un identificador únic immutable.

```csharp
public Dictionary<string, string[]> Validate()
{
    var errors = new Dictionary<string, string[]>();

    // Name: entre 3 i 100 caràcters
    if (string.IsNullOrWhiteSpace(Name) || Name.Length < 3 || Name.Length > 100)
        errors["Name"] = new[] { "Name must be between 3 and 100 characters." };

    return errors;
}
```

**Exemple de Request**:

```json
{
  "name": "Aplicació Actualitzada"
}
```

````

### 5. Pause/ResumeAppRequest

**Endpoints**:

- `PATCH /api/v1/apps/{id}/pause`
- `PATCH /api/v1/apps/{id}/resume`

```csharp
public class PauseResumeAppService
{
    // No hi ha Request, només es passa l'ID i l'email de l'usuari
    Task<IGenericResponse<int?>> PauseAsync(int appId, string userContextEmail, CancellationToken cancellationToken);
    Task<IGenericResponse<int?>> ResumeAsync(int appId, string userContextEmail, CancellationToken cancellationToken);
}
````

**Nota**: No requereix body. Només canvia `Status` a "I" (Inactiu) o "A" (Actiu).

---

## Exemples d'Ús

### Crear App (cURL)

```bash
curl -X POST https://api.nubulus.com/api/v1/apps  -H "Authorization: Bearer {token}"  -H "Content-Type: application/json"  -d '{
   "key": "my-app-key",
   "name": "Aplicació Principal"
 }'
```

**Response**: `201 Created`

### Llistar Apps (JavaScript)

```javascript
const response = await fetch(
  "https://api.nubulus.com/api/v1/apps?searchTerm=my&pageNumber=1&pageSize=20",
  {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  }
);
const data = await response.json();
console.log(`Total: ${data.totalCount}`);
console.log(data.items);
```

---

### Obtenir App per ID (cURL)

```bash
curl -X GET https://api.nubulus.com/api/v1/apps/123 \
  -H "Authorization: Bearer {token}"
```

**Response**: `200 OK`

```json
{
  "id": 123,
  "key": "my-app-key",
  "name": "Aplicació Principal",
  "status": "A"
}
```

---

### Actualitzar App (JavaScript)

```javascript
const response = await fetch("https://api.nubulus.com/api/v1/apps/123", {
  method: "PUT",
  headers: {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
  },
  body: JSON.stringify({
    name: "Aplicació Actualitzada",
  }),
});
```

**Response**: `204 No Content`

**⚠️ Nota**: Només es pot actualitzar el `Name`. La `Key` és immutable.

---

### Pausar App (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/apps/123/pause \
  -H "Authorization: Bearer {token}"
```

**Response**: `204 No Content`

**⚠️ Nota**: Això pausa l'app 123. Es canvia l'estatus a "I" (Inactiu).

---

### Reactivar App (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/apps/123/resume \
  -H "Authorization: Bearer {token}"
```

**Response**: `204 No Content`

**⚠️ Nota**: Això reactiva l'app 123. Es canvia l'estatus a "A" (Actiu).

---

## Errors i Respostes

### Codis HTTP i ResultType

| Codi HTTP          | ResultType | Descripció                    | Exemple                |
| ------------------ | ---------- | ----------------------------- | ---------------------- |
| 200 OK             | Ok         | Operació exitosa              | Obtenir app correcte   |
| 201 Created        | Ok         | Recurs creat                  | App creada             |
| 204 No Content     | Ok         | Actualització exitosa         | App actualitzada       |
| 404 Not Found      | NotFound   | Recurs no trobat              | App inexistent         |
| 409 Conflict       | Conflict   | Conflicte amb dades existents | Key duplicada          |
| 422 Unprocessable  | Problems   | Errors de validació           | Camp obligatori buit   |
| 500 Internal Error | Error      | Error del servidor            | Excepció no controlada |

---

### Exemples de Respostes d'Error

#### 1. Error de Validació (422)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "errors": {
    "Key": [
      "Key must be between 3 and 100 characters.",
      "Key can only contain letters, numbers, and hyphens."
    ],
    "Name": ["Name must be between 3 and 100 characters."]
  }
}
```

#### 2. Conflicte - Key Duplicada (409)

```json
{
  "message": "An app with the same Key already exists."
}
```

#### 3. App No Trobada (404)

```json
{
  "message": "App not found."
}
```

#### 4. Error del Servidor (500)

```json
{
  "message": "An error occurred while creating the app: {detalls}"
}
```

---

## Validació Multi-Capa

El sistema valida en **3 capes**:

### 1️⃣ Validació del Request (API)

```csharp
// En el servei
if (request.Validate().Count > 0)
    return CreateAppResponse.ValidationError(request.Validate());
```

- ✅ Format de Key (només lletres, números, guions)
- ✅ Longituds de camps
- ✅ Camps obligatoris

### 2️⃣ Validació de Negoci (Repository)

```csharp
// Verificar duplicats només en creació
var exists = await _unitOfWork.Apps.AppKeyExistsAsync(
    new AppKey(request.Key), cancellationToken);

if (exists)
    return CreateAppResponse.DataExists("An app with the same Key already exists.");
```

- ✅ Unicitat de Key (només en creació)
- ✅ Verificació en taula Apps
- ✅ **Key immutable**: No es permet actualitzar### 3️⃣ Validació de Domini (Command)

```csharp
// En el constructor del CreateApp
public CreateApp(AppKey appKey, string name)
{
    AppKey = appKey;
    Name = name;

    CreateAppValidator validator = new CreateAppValidator(this);
}

internal sealed class CreateAppValidator
{
    public CreateAppValidator(CreateApp command)
    {
        // Validació AppKey
        if (string.IsNullOrWhiteSpace(command.AppKey.Value))
            throw new ArgumentException("App key is required.");
        if (command.AppKey.Value.Length < 3 || command.AppKey.Value.Length > 100)
            throw new ArgumentException("App key must be between 3 and 100 characters.");

        // Validació Name
        if (string.IsNullOrWhiteSpace(command.Name) || command.Name.Length < 3 || command.Name.Length > 100)
            throw new ArgumentException("Name must be between 3 and 100 characters.");
    }
}
```

- ✅ Regles de negoci complexes
- ✅ Validacions amb dependències
- ✅ Llança excepcions si falla

---

## Notes Importants

### ⚠️ Autenticació

**Tots els endpoints requereixen autenticació**. L'email de l'usuari s'extreu del token:

```csharp
var userEmail = context.User.Identities.FirstOrDefault()!.Name!;
```

Aquest email s'utilitza per:

- 📝 Auditoria (qui ha fet l'acció)
- 🔐 Futura autorització (qui pot accedir a què)

### 📊 Paginació

- **Per defecte**: `pageNumber=1`, `pageSize=10`
- **Màxim**: Sense límit establert (configurable)
- **Count separat**: El total es calcula abans de la query principal

```csharp
var totalCount = await _unitOfWork.Apps.CountAppsAsync(request.SearchTerm, cancellationToken);
var appsQuery = await _unitOfWork.Apps.GetAppsAsync(request.SearchTerm, request.PageNumber, request.PageSize, cancellationToken);
```

### 🔍 Cerca (SearchTerm)

Cerca **case-insensitive** a:

- App.Name
- App.Key

```csharp
if (!string.IsNullOrWhiteSpace(searchTerm))
{
    query = query.Where(a =>
        a.Name.ToUpper().Contains(searchTerm.ToUpper()) ||
        a.Key.ToUpper().Contains(searchTerm.ToUpper()));
}
```

### 🗄️ Auditoria

Cada operació crea registres d'auditoria amb:

- **TableName**: "apps"
- **RecordType**: "Create", "Update", "Pause", "Resume"
- **UserEmail**: Qui ha executat l'acció
- **Data**: Snapshot JSON de les dades en Base64

```csharp
var appAuditRecord = app.ToAuditRecord(currentUserEmail.Value, RecordType.Create);
await _dbContext.AuditRecords.AddAsync(appAuditRecord, cancellationToken);
```

### 🔄 Pausa vs Esborrat

- **Pausar App**: Canvia `Status` a "I" (Inactive)
- **Reactivar App**: Canvia `Status` a "A" (Active)
- **No s'esborra**: Les dades es mantenen (soft delete)
- **Sense cascada**: No afecta altres entitats

⚠️ **Diferència amb Accounts**:

- Les Apps són entitats **independents**
- No tenen relacions amb altres entitats (Account, User)
- La pausa/reactivació només afecta l'app en si

### 🔑 Key vs ID

- **ID (int)**: Clau primària auto-incremental (per a relacions internes)
- **Key (string)**: Clau de negoci única (per a accés extern)
- **Key és IMMUTABLE**: Un cop creada, no es pot canviar

```csharp
// Configuració EF Core
builder.HasKey(a => a.Key);  // Key com a Primary Key
builder.HasIndex(a => a.Id);  // ID com a Index
```

**Recomanació**: Utilitzar `Key` per a URLs i referències externes, `ID` per a relacions internes.

**⚠️ Important**: Si necessites canviar la `Key`, has de crear una nova app i migrar les dades.

### 🎯 Arquitectura Clean

```
API Layer (Features/App)
  ├── CreateApp/
  │   ├── CreateAppRequest.cs      (validació format)
  │   ├── CreateAppService.cs      (lògica negoci)
  │   └── CreateAppEndPoint.cs     (routing HTTP)
  │
Domain Layer (Entities/App)
  ├── AppEntity.cs                  (entitat domini)
  ├── CreateApp.cs                  (command + validator)
  └── UpdateApp.cs                  (command + validator)
  │
Infrastructure Layer (pgsql)
  ├── Models/App.cs                 (model persistència)
  └── Repositories/AppRepository.cs (accés dades)
```

### 🔒 Value Objects

```csharp
// AppKey amb validació de format
public class AppKey
{
    public string Value { get; private set; }

    public AppKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("App key cannot be null or empty.");
        if (value.Length < 3 || value.Length > 100)
            throw new ArgumentException("App key must be between 3 and 100 characters.");

        var pattern = @"^[a-zA-Z0-9\-]+$";
        if (!Regex.IsMatch(value, pattern))
            throw new ArgumentException("App key can only contain letters, numbers, and hyphens.");

        Value = value;
    }
}
```

---

**Versió**: 1.0  
**Data**: 14 de Novembre de 2025  
**Idioma**: Català
