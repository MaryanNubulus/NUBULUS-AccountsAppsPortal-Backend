# Feature d'Users - Guia Pràctica

## Resum

Gestió d'usuaris **dins d'un Account** amb les següents funcionalitats:

- ✅ Crear usuaris associats a un Account
- ✅ Llistar usuaris d'un Account amb paginació i cerca
- ✅ Actualitzar informació d'usuaris
- ✅ Pausar/Reactivar usuaris per Account
- ✅ Compartir usuaris entre Accounts
- ✅ Validació en múltiples capes
- ✅ Auditoria completa
- ✅ Relació Account-User via taula AccountUsers

**Tecnologies**: ASP.NET Core 8 + PostgreSQL + Entity Framework Core

**⚠️ IMPORTANT**: Tots els usuaris **han de pertànyer a un Account**. Les rutes inclouen l'`accountId` com a paràmetre obligatori.

---

## Índex

1. [Endpoints](#endpoints)
2. [Estructura de Dades](#estructura-de-dades)
3. [Requests i Validacions](#requests-i-validacions)
4. [Exemples d'Ús](#exemples-dús)
5. [Errors i Respostes](#errors-i-respostes)
6. [Relació Account-User](#relació-account-user)
7. [Compartir Usuaris](#compartir-usuaris-entre-accounts)

---

## Endpoints

Tots els endpoints requereixen **autenticació** (Bearer Token).

### Resum d'Endpoints

**⚠️ Nota**: Totes les rutes inclouen `{accountId}` - els usuaris sempre pertanyen a un Account.

| Mètode | Ruta                                                  | Descripció                                    |
| ------ | ----------------------------------------------------- | --------------------------------------------- |
| POST   | `/api/v1/accounts/{accountId}/users`                  | Crear usuari en un Account                    |
| GET    | `/api/v1/accounts/{accountId}/users`                  | Llistar usuaris d'un Account                  |
| GET    | `/api/v1/accounts/{accountId}/users/{userId}`         | Obtenir usuari per ID                         |
| PUT    | `/api/v1/accounts/{accountId}/users/{userId}`         | Actualitzar usuari                            |
| PATCH  | `/api/v1/accounts/{accountId}/users/{userId}/pause`   | Pausar usuari (en tots els Accounts)          |
| PATCH  | `/api/v1/accounts/{accountId}/users/{userId}/resume`  | Reactivar usuari (en tots els Accounts)       |
| GET    | `/api/v1/accounts/{accountId}/users/to-share`         | Obtenir usuaris disponibles per compartir     |
| GET    | `/api/v1/accounts/{accountId}/users/shareds`          | Obtenir usuaris compartits amb aquest Account |
| POST   | `/api/v1/accounts/{accountId}/users/{userId}/share`   | Compartir usuari amb l'Account                |
| DELETE | `/api/v1/accounts/{accountId}/users/{userId}/unshare` | Deixar de compartir usuari                    |
| DELETE | `/api/v1/accounts/{accountId}/users/{userId}/unshare` | Deixar de compartir usuari                    |

---

## Estructura de Dades

### UserDto / UserInfoDto

Els endpoints retornen usuaris amb la següent estructura:

```json
{
  "userId": 123,
  "userKey": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "fullName": "Joan Garcia",
  "email": "joan.garcia@example.cat",
  "phone": "+34612345678",
  "status": "A",
  "isCreator": true
}
```

**Descripció dels camps**:

| Camp        | Tipus   | Descripció                                                            |
| ----------- | ------- | --------------------------------------------------------------------- |
| `userId`    | int     | ID únic de l'usuari                                                   |
| `userKey`   | string  | Clau GUID única de l'usuari (36 caràcters)                            |
| `fullName`  | string  | Nom complet de l'usuari                                               |
| `email`     | string  | Email de l'usuari (únic al sistema)                                   |
| `phone`     | string  | Telèfon de l'usuari (max 15 caràcters, required)                      |
| `status`    | string  | `"A"` = Actiu en aquest Account, `"I"` = Inactiu/Pausat               |
| `isCreator` | boolean | `true` si és el creador de l'Account, `false` si és un usuari regular |

**⚠️ Notes Importants**:

- **`status`**: És específic de la relació amb l'Account. Un mateix usuari pot estar actiu en un Account i pausat en un altre.
- **`isCreator`**: Correspon al camp `Creator = "Y"` de la taula `AccountUsers`. Només un usuari per Account pot ser creador.
- **`phone`**: Camp required amb validació de màxim 15 caràcters.
- **`userKey`**: Identificador GUID generat automàticament al crear l'usuari.

### UserToShareDto

Per als endpoints de compartir usuaris, s'utilitza un DTO més lleuger:

```json
{
  "userId": 123,
  "fullName": "Joan Garcia",
  "email": "joan.garcia@example.cat"
}
```

Aquest DTO només inclou la informació necessària per a la UI de selecció d'usuaris per compartir.

---

## Arquitectura i Patrons

### Features Agrupats

Els endpoints relacionats estan agrupats en features per funcionalitat:

#### 1. **PauseResumeUser**

Un sol servei amb dos endpoints per pausar i reactivar usuaris:

- `PauseResumeUserRequest`: Constants de ruta (`PauseRoute`, `ResumeRoute`)
- `PauseResumeUserService`: Dos mètodes (`PauseAsync`, `ResumeAsync`)
- `PauseResumeUserEndPoint`: Dos endpoints en el mateix fitxer

#### 2. **ShareUnshareUser**

Mateix patró que PauseResumeUser per compartir/deixar de compartir:

- `ShareUnshareUserRequest`: Constants de ruta (`ShareRoute`, `UnshareRoute`)
- `ShareUnshareUserService`: Dos mètodes (`ShareAsync`, `UnshareAsync`)
- `ShareUnshareUserEndPoint`: Dos endpoints en el mateix fitxer

#### 3. **GetUsersToShare** i **GetSharedUsers**

Features separats per obtenir listes d'usuaris disponibles per compartir.

**Avantatges d'aquest patró**:

- ✅ Endpoints relacionats agrupats lògicament
- ✅ Un sol servei per funcionalitats relacionades
- ✅ Manteniment més fàcil
- ✅ Consistència amb la resta de l'arquitectura

---

## Requests i Validacions

### 1. CreateUserRequest

**Endpoint**: `POST /api/v1/accounts/{accountId}/users`

```csharp
public class CreateUserRequest
{
    public int AccountId { get; init; }      // ID del Account (route parameter)
    public string FullName { get; init; }    // Nom complet de l'usuari
    public string Email { get; init; }       // Email
    public string Phone { get; init; }       // Telèfon (required, max 15 chars)
    public string? Password { get; init; }   // Contrasenya (opcional)
}
```

**Validacions**:

```csharp
public Dictionary<string, string[]> Validate()
{
    var errors = new Dictionary<string, string[]>();

    // AccountId: ha de ser > 0
    if (AccountId <= 0)
        errors["AccountId"] = new[] { "AccountId must be greater than 0." };

    // FullName: entre 2 i 100 caràcters
    if (string.IsNullOrWhiteSpace(FullName) || FullName.Length < 2 || FullName.Length > 100)
        errors["FullName"] = new[] { "FullName must be between 2 and 100 characters." };

    // Email: entre 5 i 100 caràcters, format vàlid
    if (string.IsNullOrWhiteSpace(Email) || Email.Length < 5 || Email.Length > 100)
        errors["Email"] = new[] { "Email must be between 5 and 100 characters." };
    else
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(Email, emailPattern))
            errors["Email"] = new[] { "Invalid email format." };
    }

    // Phone: required, màxim 15 caràcters
    if (string.IsNullOrWhiteSpace(Phone) || Phone.Length > 15)
        errors["Phone"] = new[] { "Phone is required and must not exceed 15 characters." };

    return errors;
}
```

**⚠️ Validació Adicional**: El servei verifica que l'Account amb `accountId` existeix abans de crear l'usuari.

**Exemple de Request**:

```json
{
  "fullName": "Joan Garcia",
  "email": "joan.garcia@example.cat",
  "phone": "+34612345678",
  "password": "secret123"
}
```

**Configuració de l'Endpoint**:

```csharp
public static class CreateUserEndPoint
{
    public static WebApplication MapCreateUserEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts/{accountId}/users", async (
            HttpContext context,
            int accountId,  // ⚠️ Route parameter obligatori
            [FromBody] CreateUserRequest request,
            [FromServices] CreateUserService service,
            CancellationToken cancellationToken) =>
        {
            // Assignar accountId del route al request
            request = request with { AccountId = accountId };

            var userEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var response = await service.ExecuteAsync(request, userEmail, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Created(),
                ResultType.NotFound => Results.NotFound(response.Message),  // Account no trobat
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateUser")
        .WithTags("Users")
        .RequireAuthorization();  // ⚠️ Autenticació obligatòria

        return app;
    }
}
```

---

### 2. GetUsersRequest

**Endpoint**: `GET /api/v1/accounts/{accountId}/users?searchTerm=Joan&pageNumber=1&pageSize=20`

```csharp
public class GetUsersRequest
{
    public int AccountId { get; set; }         // ID del Account (route parameter)
    public string? SearchTerm { get; set; }    // Cerca (opcional)
    public int? PageNumber { get; set; }       // Número de pàgina (default: 1)
    public int? PageSize { get; set; }         // Mida de pàgina (default: 10)
}
```

**⚠️ Nota**: Només retorna usuaris que pertanyen a l'Account especificat (via taula AccountUsers).

**El SearchTerm cerca a**: Name, Email

**Response**:

```json
{
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 20,
  "items": [
    {
      "userId": 1,
      "userKey": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "fullName": "Joan Garcia",
      "email": "joan.garcia@example.cat",
      "phone": "+34612345678",
      "status": "A",
      "isCreator": true
    },
    {
      "userId": 2,
      "userKey": "b2c3d4e5-f678-9012-bcde-f12345678901",
      "fullName": "Maria López",
      "email": "maria@example.cat",
      "phone": "+34623456789",
      "status": "A",
      "isCreator": false
    }
  ]
}
```

**⚠️ Notes**:

- Cerca per `FullName`, `Email` i `Phone`
- `isCreator`: Indica si l'usuari és el **creador de l'Account** (correspon al camp `Creator = "Y"` a la taula `AccountUsers`).

---

### 3. GetUserRequest

**Endpoint**: `GET /api/v1/accounts/{accountId}/users/{userId}`

```csharp
public class GetUserRequest
{
    public int AccountId { get; set; }  // ID del Account (route parameter)
    public int UserId { get; set; }     // ID de l'usuari (route parameter)
}
```

**⚠️ Validació**: El servei verifica que l'usuari pertany a l'Account abans de retornar les dades.

**Response**:

```json
{
  "userId": 123,
  "userKey": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "fullName": "Joan Garcia",
  "email": "joan.garcia@example.cat",
  "phone": "+34612345678",
  "status": "A",
  "isCreator": true
}
```

**Camps del Response**:

- `userId`: ID únic de l'usuari
- `userKey`: Clau GUID única de l'usuari
- `fullName`: Nom complet de l'usuari
- `email`: Email de l'usuari
- `phone`: Telèfon de l'usuari
- `status`: `"A"` = Actiu, `"I"` = Inactiu (Pausat)
- `isCreator`: `true` si és el creador de l'Account, `false` si no

---

### 4. UpdateUserRequest

**Endpoint**: `PUT /api/v1/accounts/{accountId}/users/{userId}`

```csharp
public class UpdateUserRequest
{
    public int AccountId { get; init; }      // ID del Account (route parameter)
    public string FullName { get; init; }    // Nom complet
    public string Email { get; init; }       // Email
    public string Phone { get; init; }       // Telèfon (required, max 15 chars)
}
```

**Validacions**:

```csharp
public Dictionary<string, string[]> Validate()
{
    var errors = new Dictionary<string, string[]>();

    // AccountId: ha de ser > 0
    if (AccountId <= 0)
        errors["AccountId"] = new[] { "AccountId must be greater than 0." };

    // FullName: entre 2 i 100 caràcters
    if (string.IsNullOrWhiteSpace(FullName) || FullName.Length < 2 || FullName.Length > 100)
        errors["FullName"] = new[] { "FullName must be between 2 and 100 characters." };

    // Email: format vàlid
    if (string.IsNullOrWhiteSpace(Email) || Email.Length < 5 || Email.Length > 100)
        errors["Email"] = new[] { "Email must be between 5 and 100 characters." };
    else
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(Email, emailPattern))
            errors["Email"] = new[] { "Invalid email format." };
    }

    // Phone: required, màxim 15 caràcters
    if (string.IsNullOrWhiteSpace(Phone) || Phone.Length > 15)
        errors["Phone"] = new[] { "Phone is required and must not exceed 15 characters." };

    return errors;
}
```

**⚠️ Validació**: El servei verifica que l'usuari pertany a l'Account abans d'actualitzar.

**Exemple de Request**:

```json
{
  "fullName": "Joan Garcia Actualitzat",
  "email": "joan.nou@example.cat",
  "phone": "+34687654321"
}
```

---

### 5. PauseResumeUserRequest

**Endpoints**:

- `PATCH /api/v1/accounts/{accountId}/users/{userId}/pause`
- `PATCH /api/v1/accounts/{accountId}/users/{userId}/resume`

```csharp
public class PauseResumeUserRequest
{
    public int AccountId { get; set; }  // ID del Account (route parameter)
    public int UserId { get; set; }     // ID de l'usuari (route parameter)
}
```

**⚠️ Important**:

- **Pausa/reactivar GLOBAL**: Afecta l'usuari a TOTS els Accounts
- L'estatus de l'usuari es canvia a la taula `Users` (camp `Status`)
- L'estatus de TOTES les relacions es canvia a la taula `AccountUsers` (camp `Status`)
- Si pausas un usuari, **es pausarà en tots els Accounts on pertany**
- Si reactives un usuari, **es reactivarà en tots els Accounts on pertany**

**No requereix body**. Només cal fer la petició PATCH a l'endpoint corresponent.

---

## Exemples d'Ús

### 1. Crear Usuari (cURL)

```bash
curl -X POST https://api.nubulus.com/api/v1/accounts/123/users \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Joan Garcia",
    "email": "joan.garcia@example.cat",
    "phone": "+34612345678",
    "password": "secret123"
  }'
```

**Response**: `201 Created`

**Errors possibles**:

- `404 Not Found`: L'Account amb ID 123 no existeix
- `409 Conflict`: Ja existeix un usuari amb aquest FullName o Email

---

### 2. Llistar Usuaris amb Cerca (JavaScript)

```javascript
const accountId = 123;
const response = await fetch(
  `https://api.nubulus.com/api/v1/accounts/${accountId}/users?searchTerm=Joan&pageNumber=1&pageSize=20`,
  {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  }
);

const data = await response.json();
console.log(`Total usuaris a l'Account ${accountId}: ${data.totalCount}`);
console.log(data.items);
```

**Nota**: La cerca inclou `FullName`, `Email` i `Phone`.

---

### 3. Obtenir Usuari per ID (C#)

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

int accountId = 123;
int userId = 456;

var response = await client.GetAsync(
    $"https://api.nubulus.com/api/v1/accounts/{accountId}/users/{userId}");

if (response.IsSuccessStatusCode)
{
    var user = await response.Content
        .ReadFromJsonAsync<UserInfoDto>();
    Console.WriteLine($"Usuari: {user.FullName}");
}
else if (response.StatusCode == HttpStatusCode.NotFound)
{
    Console.WriteLine("Account o User no trobat, o l'usuari no pertany a aquest Account");
}
```

---

### 4. Actualitzar Usuari (Python)

```python
import requests

account_id = 123
user_id = 456

url = f"https://api.nubulus.com/api/v1/accounts/{account_id}/users/{user_id}"
headers = {
    "Authorization": f"Bearer {token}",
    "Content-Type": "application/json"
}
data = {
    "fullName": "Joan Actualitzat",
    "email": "joan.nou@example.cat",
    "phone": "+34687654321"
}

response = requests.put(url, json=data, headers=headers)
print(response.status_code)  # 200 si OK, 404 si no pertany a l'Account
```

---

### 5. Pausar Usuari (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/accounts/123/users/456/pause \
  -H "Authorization: Bearer {token}"
```

**Response**: `200 OK` amb `{"data": 456}`

**⚠️ Nota**: Això pausa l'usuari 456 **globalment en TOTS els Accounts** on pertany. L'estatus de l'usuari i totes les seves relacions es canvien a "Inactiu".

---

### 6. Reactivar Usuari (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/accounts/123/users/456/resume \
  -H "Authorization: Bearer {token}"
```

**Response**: `200 OK` amb `{"data": 456}`

**⚠️ Nota**: Això reactiva l'usuari 456 **globalment en TOTS els Accounts** on pertany. L'estatus de l'usuari i totes les seves relacions es canvien a "Actiu".

---

## Errors i Respostes

### Codis HTTP i ResultType

| Codi HTTP          | ResultType | Descripció                    | Exemple                |
| ------------------ | ---------- | ----------------------------- | ---------------------- |
| 200 OK             | Ok         | Operació exitosa              | Actualització correcta |
| 201 Created        | Ok         | Recurs creat                  | Usuari creat           |
| 404 Not Found      | NotFound   | Recurs no trobat              | Usuari inexistent      |
| 409 Conflict       | Conflict   | Conflicte amb dades existents | Dades duplicades       |
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
    "FullName": ["FullName must be between 2 and 100 characters."],
    "Phone": ["Phone is required and must not exceed 15 characters."],
    "Email": ["Invalid email format."]
  }
}
```

#### 2. Conflicte - Dades Duplicades (409)

```json
{
  "message": "A user with the same Name or Email already exists."
}
```

#### 3. Usuari No Trobat (404)

```json
{
  "message": "User with ID '456' not found in Account '123'."
}
```

O bé:

```json
{
  "message": "Account with ID '123' not found."
}
```

#### 4. Error del Servidor (500)

```json
{
  "message": "An error occurred while creating the user: {detalls}"
}
```

---

## Validació Multi-Capa

El sistema valida en **3 capes**:

### 1️⃣ Validació del Request (API)

```csharp
// En el servei
if (request.Validate().Count > 0)
    return CreateUserResponse.ValidationError(request.Validate());
```

- ✅ Formats (email)
- ✅ Longituds de camps
- ✅ Camps obligatoris

### 2️⃣ Validació de Negoci (Service/Repository)

```csharp
// Verificar que l'Account existeix
var account = await _unitOfWork.Accounts.GetAccountByIdAsync(accountId);
if (account == null)
    return CreateUserResponse.NotFound("Account not found.");

// Verificar duplicats
var exists = await _unitOfWork.Users.UserInfoExistsAsync(
    request.FullName, request.Email);

if (exists)
    return CreateUserResponse.DataExists("User already exists.");

// Verificar relació Account-User
var belongsToAccount = await _unitOfWork.Users.UserBelongsToAccountAsync(
    userId, accountId);
if (!belongsToAccount)
    return GetUserResponse.NotFound("User not found in this Account.");
```

- ✅ Existència de l'Account
- ✅ Relació User-Account via taula AccountUsers
- ✅ Unicitat de FullName, Email
- ✅ Verificació en taula Users
- ✅ Exclusió d'usuari actual en actualitzacions

### 3️⃣ Validació de Domini (Command)

```csharp
// En el constructor del CreateUser
public CreateUser(/* ... */)
{
    // Assignació de propietats
    CreateUserValidator validator = new CreateUserValidator(this);
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

### 🔍 Cerca (SearchTerm)

Cerca **case-insensitive** a:

- User.FullName
- User.Email
- User.Phone

**⚠️ Nota**: La cerca **només inclou usuaris de l'Account especificat** (filtra via AccountUsers).

### 🗄️ Auditoria

Cada operació crea registres d'auditoria amb:

- **TableName**: "users"
- **RecordType**: "Create", "Update", "Pause", "Resume"
- **UserEmail**: Qui ha executat l'acció
- **Data**: Snapshot JSON de les dades

### 🔄 Pausa vs Esborrat

- **Pausar**: Modifica el camp `Status` a la taula **AccountUsers** (no a Users)
- **Per Account**: Un usuari pot estar pausat en un Account i actiu en un altre
- **No s'esborra**: Les dades es mantenen (soft delete conceptual)
- **Auditoria**: Crea registre d'auditoria de tipus "Pause" o "Resume"
- **Status**: `"A"` = Actiu, `"I"` = Inactiu (Pausat)

---

## Relació Account-User

### Taula AccountUsers

La relació entre Accounts i Users es gestiona mitjançant la taula **AccountUsers**:

```sql
CREATE TABLE account_users (
    key VARCHAR(36) PRIMARY KEY,
    account_key VARCHAR(36) NOT NULL,
    user_key VARCHAR(36) NOT NULL,
    creator CHAR(1) NOT NULL DEFAULT 'N',  -- 'Y' = creador, 'N' = usuari normal
    status CHAR(1) NOT NULL DEFAULT 'A',   -- 'A' = actiu, 'I' = inactiu
    FOREIGN KEY (account_key) REFERENCES accounts(key),
    FOREIGN KEY (user_key) REFERENCES users(key)
);
```

### Característiques Clau

1. **Many-to-Many**: Un usuari pot pertànyer a múltiples Accounts
2. **Status per Account**: L'estat (actiu/pausat) és específic de cada relació
3. **Creator Flag**: Identifica qui va crear l'Account (`Creator = "Y"`)
   - Només **un usuari per Account** té `Creator = "Y"`
   - Aquest valor es retorna com `isCreator: true` en els DTOs
   - Els altres usuaris tenen `Creator = "N"` i `isCreator: false`
4. **Obligatori**: **Tots els usuaris han de pertànyer almenys a un Account**

### Camp `isCreator` en les Respostes

El camp `isCreator` que es retorna en els endpoints prové directament de `AccountUsers.Creator`:

```csharp
// Al UserRepository
IsCreator = x.AccountUser.Creator == "Y"
```

**Casos d'ús**:

- ✅ Identificar l'administrador principal de l'Account
- ✅ Mostrar icones o badges especials en la UI
- ✅ Aplicar permisos especials (futura funcionalitat)
- ✅ Evitar pausar o eliminar el creador sense transferir propietat

### ParentKey - Tracking de l'Account Original

Cada usuari té un camp `ParentKey` que identifica **l'Account que originalment va crear l'usuari**:

```csharp
public class UserEntity
{
    public AccountKey ParentKey { get; set; }  // Account que va crear aquest usuari
    // ... altres camps
}
```

**Propòsit del ParentKey**:

- 🔍 **Tracking d'origen**: Saber quin Account va crear originalment l'usuari
- 🔗 **Compartir usuaris**: Permet distingir entre usuaris propis i compartits
- 📊 **Reporting**: Facilita estadístiques sobre creació i compartició d'usuaris

**Diferència entre ParentKey i AccountUsers**:

| Aspecte      | `ParentKey`                       | `AccountUsers`                                  |
| ------------ | --------------------------------- | ----------------------------------------------- |
| Definició    | Account que va **crear** l'usuari | Accounts amb els que l'usuari està **vinculat** |
| Cardinalitat | **Un** sol valor (no canvia mai)  | **Múltiples** relacions (many-to-many)          |
| Ús           | Identificar propietari original   | Gestionar accés i permisos                      |
| Exemple      | `ParentKey = "account-123"`       | AccountUsers: `["account-123", "account-456"]`  |

**Exemple pràctic**:

1. Account A crea l'usuari Joan → `ParentKey = "account-a-key"`
2. Joan es comparteix amb Account B → AccountUsers: `["account-a-key", "account-b-key"]`
3. Joan es comparteix amb Account C → AccountUsers: `["account-a-key", "account-b-key", "account-c-key"]`
4. El `ParentKey` sempre serà `"account-a-key"` (no canvia mai)

---

## Compartir Usuaris entre Accounts

Els usuaris poden ser **compartits** entre diferents Accounts mitjançant la funcionalitat de sharing.

### GET - Obtenir Usuaris Disponibles per Compartir (amb Paginació)

**Endpoint**: `GET /api/v1/accounts/{accountId}/users/to-share`

Retorna una llista d'usuaris que **NO** estan vinculats a l'Account especificat i que poden ser compartits, amb suport per a **paginació i cerca**.

**Paràmetres de Query**:

| Paràmetre    | Tipus  | Obligatori | Descripció                       |
| ------------ | ------ | ---------- | -------------------------------- |
| `searchTerm` | string | No         | Filtra per nom o email           |
| `pageNumber` | int    | No         | Número de pàgina (default: 1)    |
| `pageSize`   | int    | No         | Usuaris per pàgina (default: 10) |

**Exemple de Request**:

```
GET /api/v1/accounts/1/users/to-share?searchTerm=maria&pageNumber=1&pageSize=10
```

**Resposta**:

```json
{
  "totalCount": 15,
  "pageNumber": 1,
  "pageSize": 10,
  "items": [
    {
      "userId": 45,
      "fullName": "Maria Lopez",
      "email": "maria.lopez@example.cat"
    },
    {
      "userId": 67,
      "fullName": "Maria García",
      "email": "maria.garcia@example.cat"
    }
  ]
}
```

**Estructura de la Resposta** (`PaginatedResponse<UserToShareDto>`):

| Camp         | Tipus              | Descripció                       |
| ------------ | ------------------ | -------------------------------- |
| `totalCount` | int                | Total d'usuaris que coincideixen |
| `pageNumber` | int                | Número de pàgina actual          |
| `pageSize`   | int                | Usuaris per pàgina               |
| `items`      | `UserToShareDto[]` | Array d'usuaris disponibles      |

**Lògica**:

```csharp
// Retorna usuaris que NO tenen relació AccountUser amb aquest Account
var usersAlreadyInAccount = _dbContext.AccountUsers
    .Where(au => au.AccountKey == account.Key)
    .Select(au => au.UserKey);

var availableUsers = _dbContext.Users
    .Where(u => !usersAlreadyInAccount.Contains(u.Key));

// Aplicar filtre de cerca si es proporciona
if (!string.IsNullOrWhiteSpace(searchTerm))
{
    availableUsers = availableUsers.Where(u =>
        u.FullName.Contains(searchTerm) ||
        u.Email.Contains(searchTerm));
}

// Aplicar paginació
var paginated = availableUsers
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```

### GET - Obtenir Usuaris Compartits (amb Paginació)

**Endpoint**: `GET /api/v1/accounts/{accountId}/users/shareds`

Retorna usuaris compartits amb aquest Account (on `ParentKey != accountKey`) amb suport per a **paginació i cerca**.

**Paràmetres de Query**:

| Paràmetre    | Tipus  | Obligatori | Descripció                       |
| ------------ | ------ | ---------- | -------------------------------- |
| `searchTerm` | string | No         | Filtra per nom o email           |
| `pageNumber` | int    | No         | Número de pàgina (default: 1)    |
| `pageSize`   | int    | No         | Usuaris per pàgina (default: 10) |

**Exemple de Request**:

```
GET /api/v1/accounts/1/users/shareds?searchTerm=maria&pageNumber=1&pageSize=10
```

**Resposta**:

```json
{
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "items": [
    {
      "userId": 45,
      "fullName": "Maria Lopez",
      "email": "maria.lopez@example.cat"
    },
    {
      "userId": 67,
      "fullName": "Maria García",
      "email": "maria.garcia@example.cat"
    }
  ]
}
```

**Estructura de la Resposta** (`PaginatedResponse<UserToShareDto>`):

| Camp         | Tipus              | Descripció                       |
| ------------ | ------------------ | -------------------------------- |
| `totalCount` | int                | Total d'usuaris que coincideixen |
| `pageNumber` | int                | Número de pàgina actual          |
| `pageSize`   | int                | Usuaris per pàgina               |
| `items`      | `UserToShareDto[]` | Array d'usuaris compartits       |

**Lògica**:

```csharp
// Només retorna usuaris on ParentKey és diferent del Account actual
var sharedUsers = from u in _dbContext.Users
                  join au in _dbContext.AccountUsers on u.Key equals au.UserKey
                  where au.AccountKey == account.Key && u.ParentKey != account.Key
                  select u;

// Aplicar filtre de cerca si es proporciona
if (!string.IsNullOrWhiteSpace(searchTerm))
{
    sharedUsers = sharedUsers.Where(u =>
        u.FullName.Contains(searchTerm) ||
        u.Email.Value.Contains(searchTerm));
}

// Aplicar paginació
var paginated = sharedUsers
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```

### POST - Compartir Usuari i DELETE - Deixar de Compartir

Els endpoints de compartir i deixar de compartir estan implementats en el feature `ShareUnshareUser` seguint el mateix patró que `PauseResumeUser` (dos endpoints en un sol fitxer).

#### ShareUnshareUserRequest

```csharp
public class ShareUnshareUserRequest
{
    public const string ShareRoute = "/api/v1/accounts/{accountId}/users/{userId}/share";
    public const string UnshareRoute = "/api/v1/accounts/{accountId}/users/{userId}/unshare";
    public int AccountId { get; set; }
    public int UserId { get; set; }
}
```

#### ShareUnshareUserService

```csharp
public class ShareUnshareUserService
{
    public async Task<IGenericResponse<string>> ShareAsync(int accountId, int userId, string userContextEmail, CancellationToken cancellationToken)
    { /* Compartir usuari */ }

    public async Task<IGenericResponse<string>> UnshareAsync(int accountId, int userId, string userContextEmail, CancellationToken cancellationToken)
    { /* Deixar de compartir */ }
}
```

#### Compartir Usuari

**Endpoint**: `POST /api/v1/accounts/{accountId}/users/{userId}/share`

Comparteix un usuari existent amb un Account diferent.

**Body**: _(vacio)_

**Resposta**:

```json
{
  "message": "User shared successfully."
}
```

**Lògica**:

1. Verifica que l'usuari existeix
2. Verifica que l'Account existeix
3. Comprova que **no existeix ja la relació**
4. Crea una nova relació `AccountUser` amb:
   - `Creator = "N"` (no és el creador)
   - `Status = "A"` (actiu per defecte)
5. Registra l'acció en auditoria

**Validacions**:

- ❌ `404 Not Found`: Usuari o Account no trobat
- ❌ `409 Conflict`: Usuari ja està compartit amb aquest Account
- ✅ `200 OK`: Usuari compartit correctament

#### Deixar de Compartir Usuari

**Endpoint**: `DELETE /api/v1/accounts/{accountId}/users/{userId}/unshare`

Elimina la relació de compartició entre un usuari i un Account.

**Resposta**:

```json
{
  "message": "User unshared successfully."
}
```

**Lògica**:

1. Verifica que existeix la relació `AccountUser`
2. **NO permet eliminar** si `Creator = "Y"` (no pots deixar de compartir el creador)
3. Elimina la relació `AccountUser`
4. Registra l'acció en auditoria amb `RecordType.Delete`

**Validacions**:

- ❌ `404 Not Found`: Usuari o Account no trobat, o no hi ha relació
- ❌ `409 Conflict`: No es pot deixar de compartir el creador de l'Account
- ✅ `200 OK`: Relació eliminada correctament

### Exemple Complet de Compartir Usuaris

**Escenari**: Account B vol compartir l'usuari Maria (creat per Account A)

1. **Obtenir usuaris disponibles**:

```bash
GET /api/v1/accounts/2/users/to-share
```

Resposta:

```json
[
  {
    "userId": 45,
    "fullName": "Maria Lopez",
    "email": "maria.lopez@example.cat"
  }
]
```

2. **Compartir l'usuari Maria amb Account B**:

```bash
POST /api/v1/accounts/2/users/45/share
```

Resposta:

```json
{
  "message": "User shared successfully."
}
```

3. **Verificar usuaris compartits**:

```bash
GET /api/v1/accounts/2/users/shareds
```

Resposta:

```json
[
  {
    "userId": 45,
    "fullName": "Maria Lopez",
    "email": "maria.lopez@example.cat"
  }
]
```

4. **Deixar de compartir**:

```bash
DELETE /api/v1/accounts/2/users/45/unshare
```

**⚠️ Regles Importants**:

- No pots deixar de compartir un usuari amb `isCreator = true`
- Els usuaris compartits tenen `isCreator = false` automàticament
- El `ParentKey` de l'usuari mai canvia (sempre apunta a l'Account creador)
- Compartir crea una relació `AccountUser` amb `Creator = "N"`

### Diferències amb Accounts

Els **Users** són més simples que els **Accounts**:

1. **No tenen camps addicionals** com Phone, Address, NumberId
2. **Sempre pertanyen a un Account** - relació obligatòria via AccountUsers
3. **Status en AccountUsers** - no directament a la taula Users
4. **Més lleugers** - només Name i Email
5. **Reutilitzables** - un mateix usuari pot estar en diversos Accounts
6. **ParentKey** - Tracking de l'Account que va crear l'usuari originalment

---

**Versió**: 1.3  
**Data**: 13 de Novembre de 2025  
**Idioma**: Català  
**Última actualització**: Refactorització: ShareUnshareUser ara segueix el patró PauseResumeUser (dos endpoints en un sol servei)
