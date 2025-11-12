# Feature d'Users - Guia Pràctica

## Resum

Gestió d'usuaris **dins d'un Account** amb les següents funcionalitats:

- ✅ Crear usuaris associats a un Account
- ✅ Llistar usuaris d'un Account amb paginació i cerca
- ✅ Actualitzar informació d'usuaris
- ✅ Pausar/Reactivar usuaris per Account
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

---

## Endpoints

Tots els endpoints requereixen **autenticació** (Bearer Token).

### Resum d'Endpoints

**⚠️ Nota**: Totes les rutes inclouen `{accountId}` - els usuaris sempre pertanyen a un Account.

| Mètode | Ruta                                                 | Descripció                         |
| ------ | ---------------------------------------------------- | ---------------------------------- |
| POST   | `/api/v1/accounts/{accountId}/users`                 | Crear usuari en un Account         |
| GET    | `/api/v1/accounts/{accountId}/users`                 | Llistar usuaris d'un Account       |
| GET    | `/api/v1/accounts/{accountId}/users/{userId}`        | Obtenir usuari per ID              |
| PUT    | `/api/v1/accounts/{accountId}/users/{userId}`        | Actualitzar usuari                 |
| PATCH  | `/api/v1/accounts/{accountId}/users/{userId}/pause`  | Pausar usuari en aquest Account    |
| PATCH  | `/api/v1/accounts/{accountId}/users/{userId}/resume` | Reactivar usuari en aquest Account |

---

## Estructura de Dades

### UserDto / UserInfoDto

Els endpoints retornen usuaris amb la següent estructura:

```json
{
  "userId": 123,
  "name": "Joan Garcia",
  "email": "joan.garcia@example.cat",
  "status": "A",
  "isCreator": true
}
```

**Descripció dels camps**:

| Camp        | Tipus   | Descripció                                                            |
| ----------- | ------- | --------------------------------------------------------------------- |
| `userId`    | int     | ID únic de l'usuari                                                   |
| `name`      | string  | Nom complet de l'usuari                                               |
| `email`     | string  | Email de l'usuari (únic al sistema)                                   |
| `status`    | string  | `"A"` = Actiu en aquest Account, `"I"` = Inactiu/Pausat               |
| `isCreator` | boolean | `true` si és el creador de l'Account, `false` si és un usuari regular |

**⚠️ Notes Importants**:

- **`status`**: És específic de la relació amb l'Account. Un mateix usuari pot estar actiu en un Account i pausat en un altre.
- **`isCreator`**: Correspon al camp `Creator = "Y"` de la taula `AccountUsers`. Només un usuari per Account pot ser creador.

---

## Requests i Validacions

### 1. CreateUserRequest

**Endpoint**: `POST /api/v1/accounts/{accountId}/users`

```csharp
public class CreateUserRequest
{
    public int AccountId { get; init; }   // ID del Account (route parameter)
    public string Name { get; init; }     // Nom de l'usuari
    public string Email { get; init; }    // Email
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

    // Name: entre 2 i 100 caràcters
    if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2 || Name.Length > 100)
        errors["Name"] = new[] { "Name must be between 2 and 100 characters." };

    // Email: entre 5 i 100 caràcters, format vàlid
    if (string.IsNullOrWhiteSpace(Email) || Email.Length < 5 || Email.Length > 100)
        errors["Email"] = new[] { "Email must be between 5 and 100 characters." };
    else
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(Email, emailPattern))
            errors["Email"] = new[] { "Invalid email format." };
    }

    return errors;
}
```

**⚠️ Validació Adicional**: El servei verifica que l'Account amb `accountId` existeix abans de crear l'usuari.

**Exemple de Request**:

```json
{
  "name": "Joan Garcia",
  "email": "joan.garcia@example.cat"
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
      "name": "Joan Garcia",
      "email": "joan.garcia@example.cat",
      "status": "A",
      "isCreator": true
    },
    {
      "userId": 2,
      "name": "Maria López",
      "email": "maria@example.cat",
      "status": "A",
      "isCreator": false
    }
  ]
}
```

**⚠️ Nota sobre `isCreator`**: Indica si l'usuari és el **creador de l'Account** (correspon al camp `Creator = "Y"` a la taula `AccountUsers`).

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
  "name": "Joan Garcia",
  "email": "joan.garcia@example.cat",
  "status": "A",
  "isCreator": true
}
```

**Camps del Response**:

- `userId`: ID únic de l'usuari
- `name`: Nom de l'usuari
- `email`: Email de l'usuari
- `status`: `"A"` = Actiu, `"I"` = Inactiu (Pausat)
- `isCreator`: `true` si és el creador de l'Account, `false` si no

---

### 4. UpdateUserRequest

**Endpoint**: `PUT /api/v1/accounts/{accountId}/users/{userId}`

```csharp
public class UpdateUserRequest
{
    public int AccountId { get; init; }   // ID del Account (route parameter)
    public string Name { get; init; }
    public string Email { get; init; }
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

    // Name: entre 2 i 100 caràcters
    if (string.IsNullOrWhiteSpace(Name) || Name.Length < 2 || Name.Length > 100)
        errors["Name"] = new[] { "Name must be between 2 and 100 characters." };

    // Email: format vàlid
    if (string.IsNullOrWhiteSpace(Email) || Email.Length < 5 || Email.Length > 100)
        errors["Email"] = new[] { "Email must be between 5 and 100 characters." };
    else
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(Email, emailPattern))
            errors["Email"] = new[] { "Invalid email format." };
    }

    return errors;
}
```

**⚠️ Validació**: El servei verifica que l'usuari pertany a l'Account abans d'actualitzar.

**Exemple de Request**:

```json
{
  "name": "Joan Garcia Actualitzat",
  "email": "joan.nou@example.cat"
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

- Pausa/reactivar **només afecta la relació** entre l'usuari i aquest Account específic
- L'estat es guarda a la taula `AccountUsers` (camp `Status`)
- Un mateix usuari pot estar actiu en un Account i pausat en un altre

**No requereix body**. Només cal fer la petició PATCH a l'endpoint corresponent.

---

## Exemples d'Ús

### 1. Crear Usuari (cURL)

```bash
curl -X POST https://api.nubulus.com/api/v1/accounts/123/users \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Joan Garcia",
    "email": "joan.garcia@example.cat"
  }'
```

**Response**: `201 Created`

**Errors possibles**:

- `404 Not Found`: L'Account amb ID 123 no existeix
- `409 Conflict`: Ja existeix un usuari amb aquest Name o Email

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

**Nota**: Només retorna usuaris associats a l'Account 123.

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
    Console.WriteLine($"Usuari: {user.Name}");
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
    "name": "Joan Actualitzat",
    "email": "joan.nou@example.cat"
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

**⚠️ Nota**: Això pausa l'usuari 456 **només per l'Account 123**. Si l'usuari pertany a altres Accounts, romandran sense canvis.

---

### 6. Reactivar Usuari (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/accounts/123/users/456/resume \
  -H "Authorization: Bearer {token}"
```

**Response**: `200 OK` amb `{"data": 456}`

---

## Errors i Respostes

### Codis HTTP i ResultType

| Codi HTTP          | ResultType | Descripció                    | Exemple                |
| ------------------ | ---------- | ----------------------------- | ---------------------- |
| 200 OK             | Ok         | Operació exitosa              | Actualització correcta |
| 201 Created        | Ok         | Recurs creat                  | Usuari creat           |
| 404 Not Found      | NotFound   | Recurs no trobat              | Usuari inexistent      |
| 409 Conflict       | Conflict   | Conflicte amb dades existents | Email duplicat         |
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
    "Name": ["Name must be between 2 and 100 characters."],
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
    request.Name, request.Email);

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
- ✅ Unicitat de Name, Email
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

- User.Name
- User.Email

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

### Diferències amb Accounts

Els **Users** són més simples que els **Accounts**:

1. **No tenen camps addicionals** com Phone, Address, NumberId
2. **Sempre pertanyen a un Account** - relació obligatòria via AccountUsers
3. **Status en AccountUsers** - no directament a la taula Users
4. **Més lleugers** - només Name i Email
5. **Reutilitzables** - un mateix usuari pot estar en diversos Accounts

---

**Versió**: 1.1  
**Data**: 12 de Novembre de 2025  
**Idioma**: Català  
**Última actualització**: Afegit camp `isCreator` per identificar el creador de l'Account
