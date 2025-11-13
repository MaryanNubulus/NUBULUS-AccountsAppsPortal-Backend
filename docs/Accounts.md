# Feature d'Accounts - Guia Pràctica

## Resum

Gestió de comptes (organitzacions/empreses) amb les següents funcionalitats:

- ✅ Crear comptes amb usuari creador automàtic
- ✅ Llistar comptes amb paginació i cerca
- ✅ Actualitzar informació de comptes
- ✅ Pausar/Reactivar comptes
- ✅ Validació en múltiples capes
- ✅ Auditoria completa

**Tecnologies**: ASP.NET Core 8 + PostgreSQL + Entity Framework Core

---

## Índex

1. [Endpoints](#endpoints)
2. [Requests i Validacions](#requests-i-validacions)
3. [Exemples d'Ús](#exemples-dús)
4. [Errors i Respostes](#errors-i-respostes)

---

## Endpoints

Tots els endpoints requereixen **autenticació** (Bearer Token).

### Resum d'Endpoints

| Mètode | Ruta                           | Descripció                |
| ------ | ------------------------------ | ------------------------- |
| POST   | `/api/v1/accounts`             | Crear compte              |
| GET    | `/api/v1/accounts`             | Llistar comptes (paginat) |
| GET    | `/api/v1/accounts/{id}`        | Obtenir compte per ID     |
| PUT    | `/api/v1/accounts/{id}`        | Actualitzar compte        |
| PATCH  | `/api/v1/accounts/{id}/pause`  | Pausar compte             |
| PATCH  | `/api/v1/accounts/{id}/resume` | Reactivar compte          |

---

## Requests i Validacions

### 1. CreateAccountRequest

**Endpoint**: `POST /api/v1/accounts`

```csharp
public class CreateAccountRequest
{
    public string Name { get; init; }         // Nom del compte
    public string FullName { get; init; }     // Nom complet del creador
    public string Email { get; init; }        // Email
    public string Phone { get; init; }        // Telèfon
    public string Address { get; init; }      // Adreça
    public string NumberId { get; init; }     // Número d'identificació
}
```

**Validacions**:

```csharp
public Dictionary<string, string[]> Validate()
{
    var errors = new Dictionary<string, string[]>();

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

    // Phone: entre 10 i 15 caràcters, només números i opcional +
    if (string.IsNullOrWhiteSpace(Phone) || Phone.Length < 10 || Phone.Length > 15)
        errors["Phone"] = new[] { "Phone must be between 10 and 15 characters." };
    else
    {
        var phonePattern = @"^\+?[0-9]{10,15}$";
        if (!Regex.IsMatch(Phone, phonePattern))
            errors["Phone"] = new[] { "Invalid phone number format." };
    }

    // Address: entre 5 i 200 caràcters
    if (string.IsNullOrWhiteSpace(Address) || Address.Length < 5 || Address.Length > 200)
        errors["Address"] = new[] { "Address must be between 5 and 200 characters." };

    // NumberId: entre 5 i 50 caràcters
    if (string.IsNullOrWhiteSpace(NumberId) || NumberId.Length < 5 || NumberId.Length > 50)
        errors["NumberId"] = new[] { "NumberId must be between 5 and 50 characters." };

    return errors;
}
```

**Exemple de Request**:

```json
{
  "name": "ACME Corporation",
  "fullName": "Joan Garcia",
  "email": "contacte@acme.cat",
  "phone": "+34612345678",
  "address": "Carrer Major 123, Barcelona",
  "numberId": "B12345678"
}
```

**Configuració de l'Endpoint**:

```csharp
public static class CreateAccountEndPoint
{
    public static WebApplication MapCreateAccountEndPoint(this WebApplication app)
    {
        app.MapPost("/api/v1/accounts", async (
            HttpContext context,
            [FromBody] CreateAccountRequest request,
            [FromServices] CreateAccountService service,
            CancellationToken cancellationToken) =>
        {
            var userEmail = context.User.Identities.FirstOrDefault()!.Name!;
            var response = await service.ExecuteAsync(request, userEmail, cancellationToken);

            return response.ResultType switch
            {
                ResultType.Ok => Results.Created(),
                ResultType.Conflict => Results.Conflict(response.Message),
                ResultType.Problems => Results.ValidationProblem(response.ValidationErrors!),
                _ => Results.Problem(response.Message)
            };
        })
        .WithName("CreateAccount")
        .WithTags("Accounts")
        .RequireAuthorization();  // ⚠️ Autenticació obligatòria

        return app;
    }
}
```

---

### 2. GetAccountsRequest

**Endpoint**: `GET /api/v1/accounts?searchTerm=ACME&pageNumber=1&pageSize=20`

```csharp
public class GetAccountsRequest
{
    public string? SearchTerm { get; set; }    // Cerca (opcional)
    public int? PageNumber { get; set; }       // Número de pàgina (default: 1)
    public int? PageSize { get; set; }         // Mida de pàgina (default: 10)
}
```

**El SearchTerm cerca a**: Name, Email, Phone, NumberId, FullName del creador

**Response**:

```json
{
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 20,
  "items": [
    {
      "accountId": 1,
      "name": "ACME Corporation",
      "fullName": "Joan Garcia",
      "email": "contacte@acme.cat",
      "phone": "+34612345678",
      "numberId": "B12345678",
      "status": "A"
    }
  ]
}
```

---

### 3. GetAccountRequest

**Endpoint**: `GET /api/v1/accounts/{accountId}`

```csharp
public class GetAccountRequest
{
    public int AccountId { get; set; }   // ID del compte (route parameter)
}
```

**Response** (inclou més detalls que el llistat):

```json
{
  "accountId": 123,
  "name": "ACME Corporation",
  "fullName": "Joan Garcia",
  "email": "contacte@acme.cat",
  "phone": "+34612345678",
  "numberId": "B12345678",
  "address": "Carrer Major 123, Barcelona", // 👈 Camp addicional
  "status": "A"
}
```

---

### 4. UpdateAccountRequest

**Endpoint**: `PUT /api/v1/accounts/{accountId}`

```csharp
public class UpdateAccountRequest
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    public string Address { get; init; }
    public string NumberId { get; init; }
}
```

**Validacions**:

```csharp
public Dictionary<string, string[]> Validate()
{
    var errors = new Dictionary<string, string[]>();

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

    // Phone: format vàlid
    if (string.IsNullOrWhiteSpace(Phone) || Phone.Length < 10 || Phone.Length > 15)
        errors["Phone"] = new[] { "Phone must be between 10 and 15 characters." };
    else
    {
        var phonePattern = @"^\+?[0-9]{10,15}$";
        if (!Regex.IsMatch(Phone, phonePattern))
            errors["Phone"] = new[] { "Invalid phone format." };
    }

    // Address: opcional, màxim 200 caràcters
    if (!string.IsNullOrWhiteSpace(Address) && Address.Length > 200)
        errors["Address"] = new[] { "Address must not exceed 200 characters." };

    // NumberId: opcional, màxim 50 caràcters
    if (!string.IsNullOrWhiteSpace(NumberId) && NumberId.Length > 50)
        errors["NumberId"] = new[] { "NumberId must not exceed 50 characters." };

    return errors;
}
```

**Exemple de Request**:

```json
{
  "name": "ACME Corp Actualitzat",
  "email": "nou@acme.cat",
  "phone": "+34612345679",
  "address": "Nova Adreça 456",
  "numberId": "B12345678"
}
```

---

### 5. PauseResumeAccountRequest

**Endpoints**:

- `PATCH /api/v1/accounts/{accountId}/pause`
- `PATCH /api/v1/accounts/{accountId}/resume`

```csharp
public class PauseResumeAccountRequest
{
    public int AccountId { get; set; }   // Route parameter
}
```

**No requereix body**. Només cal fer la petició PATCH a l'endpoint corresponent.

---

## Exemples d'Ús

### 1. Crear Compte (cURL)

```bash
curl -X POST https://api.nubulus.com/api/v1/accounts \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "ACME Corporation",
    "fullName": "Joan Garcia",
    "email": "contacte@acme.cat",
    "phone": "+34612345678",
    "address": "Carrer Major 123, Barcelona",
    "numberId": "B12345678"
  }'
```

**Response**: `201 Created`

---

### 2. Llistar Comptes amb Cerca (JavaScript)

```javascript
const response = await fetch(
  "https://api.nubulus.com/api/v1/accounts?searchTerm=ACME&pageNumber=1&pageSize=20",
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

### 3. Obtenir Compte per ID (C#)

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var response = await client.GetAsync(
    "https://api.nubulus.com/api/v1/accounts/123");

if (response.IsSuccessStatusCode)
{
    var account = await response.Content
        .ReadFromJsonAsync<AccountInfoDto>();
    Console.WriteLine($"Compte: {account.Name}");
}
```

---

### 4. Actualitzar Compte (Python)

```python
import requests

url = "https://api.nubulus.com/api/v1/accounts/123"
headers = {
    "Authorization": f"Bearer {token}",
    "Content-Type": "application/json"
}
data = {
    "name": "ACME Actualitzat",
    "email": "nou@acme.cat",
    "phone": "+34612345679",
    "address": "Nova Adreça",
    "numberId": "B12345678"
}

response = requests.put(url, json=data, headers=headers)
print(response.status_code)  # 200
```

---

### 5. Pausar Compte (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/accounts/123/pause \
  -H "Authorization: Bearer {token}"
```

**Response**: `200 OK` amb `{"data": 123}`

**⚠️ Nota**: Això pausa el compte 123. Es canvia l'estatus del compte a "Inactiu" i **TOTES les seves relacions AccountUsers es pausan**.

---

### 6. Reactivar Compte (cURL)

```bash
curl -X PATCH https://api.nubulus.com/api/v1/accounts/123/resume \
  -H "Authorization: Bearer {token}"
```

**Response**: `200 OK` amb `{"data": 123}`

**⚠️ Nota**: Això reactiva el compte 123. Es canvia l'estatus del compte a "Actiu" i **TOTES les seves relacions AccountUsers es reactiven**.

---

## Errors i Respostes

### Codis HTTP i ResultType

| Codi HTTP          | ResultType | Descripció                    | Exemple                |
| ------------------ | ---------- | ----------------------------- | ---------------------- |
| 200 OK             | Ok         | Operació exitosa              | Actualització correcta |
| 201 Created        | Ok         | Recurs creat                  | Compte creat           |
| 404 Not Found      | NotFound   | Recurs no trobat              | Compte inexistent      |
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
    "Name": ["Name must be between 2 and 100 characters."],
    "Email": ["Invalid email format."],
    "Phone": ["Invalid phone number format."]
  }
}
```

#### 2. Conflicte - Dades Duplicades (409)

```json
{
  "message": "An account with the same Name, Email, Phone, or NumberId already exists."
}
```

#### 3. Compte No Trobat (404)

```json
{
  "message": "Account with ID '123' not found."
}
```

#### 4. Error del Servidor (500)

```json
{
  "message": "An error occurred while creating the account: {detalls}"
}
```

---

## Validació Multi-Capa

El sistema valida en **3 capes**:

### 1️⃣ Validació del Request (API)

```csharp
// En el servei
if (request.Validate().Count > 0)
    return CreateAccountResponse.ValidationError(request.Validate());
```

- ✅ Formats (email, telèfon)
- ✅ Longituds de camps
- ✅ Camps obligatoris

### 2️⃣ Validació de Negoci (Repository)

```csharp
// Verificar duplicats
var exists = await _unitOfWork.Accounts.AccountInfoExistsAsync(
    request.Name, request.Email, request.Phone, request.NumberId);

if (exists)
    return CreateAccountResponse.DataExists("Account already exists.");
```

- ✅ Unicitat de Name, Email, Phone, NumberId
- ✅ Verificació en taula Accounts **i** Users
- ✅ Exclusió de compte actual en actualitzacions

### 3️⃣ Validació de Domini (Command)

```csharp
// En el constructor del CreateAccount
public CreateAccount(/* ... */)
{
    // Assignació de propietats
    CreateAccountValidator validator = new CreateAccountValidator(this);
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

- Account.Name
- Account.Email
- Account.Phone
- Account.NumberId
- User.Name (nom complet del creador)

### 🗄️ Auditoria

Cada operació crea registres d'auditoria amb:

- **TableName**: "accounts", "users", "account_users"
- **RecordType**: "Create", "Update", "Pause", "Resume"
- **UserEmail**: Qui ha executat l'acció
- **Data**: Snapshot JSON de les dades

### 🔄 Pausa vs Esborrat

- **Pausar Compte**: Canvia `Status` a "I" (Inactive) i pausa **TOTES les relacions AccountUsers**
- **Pausar Usuari**: Canvia `Status` a "I" (Inactive) i pausa **TOTES les relacions AccountUsers de l'usuari**
- **No s'esborra**: Les dades es mantenen (soft delete)
- **Efecte cascada**:
  - Pausa de compte → pausa les relacions dels usuaris d'aquell compte
  - Pausa de usuari → pausa les relacions de l'usuari en TOTS els comptes
  - Els usuaris es pausan de manera independent del compte

⚠️ **Diferència important**:

- L'estatus del **Compte** afecta les relacions `AccountUsers`
- L'estatus de l'**Usuari** afecta els registres de `User` i totes les seves relacions `AccountUsers` globalment

---

**Versió**: 1.0  
**Data**: 12 de Novembre de 2025  
**Idioma**: Català
