# CreateUser — Crear un usuari

Aquest README descriu la feature `CreateUser` que permet crear un nou usuari a la base de dades.

Fitxers rellevants

- `ICreateUserService.cs` — interfície pública amb la signatura del servei.
- `CreateUserService.cs` — implementació que utilitza `IUsersCommandsRepository`.

Contracte

- Mètode públic: `Task<bool> CreateUserAsync(string email, string name)`
  - Inputs:
    - `email` (string) — correu electrònic de l'usuari.
    - `name` (string) — nom de l'usuari.
  - Output: `Task<bool>` — `true` si la creació s'ha completat amb èxit, `false` en cas contrari.
  - Errors: es propaguen excepcions del repositori (errors de BD, violacions d'índexs, etc.).

Com funciona

1. `CreateUserService` rep via constructor una instància de `IUsersCommandsRepository`.
2. Crea una entitat `User` amb `User.Create(email, name)` (fàbrica/constructor de domini).
3. Crida `_usersCommandsRepository.AddAsync(user)` i retorna el resultat (bool) que indica si l'afegit ha tingut èxit.

Exemple d'injecció DI

```csharp
// Registrar serveis
services.AddScoped<IUsersCommandsRepository, UsersCommandsRepository>();
services.AddScoped<ICreateUserService, CreateUserService>();
```

Ús típic

```csharp
public class UsersController : ControllerBase
{
    private readonly ICreateUserService _createUserService;

    public UsersController(ICreateUserService createUserService)
    {
        _createUserService = createUserService;
    }

    public async Task<IActionResult> Register(UserDto dto)
    {
        var created = await _createUserService.CreateUserAsync(dto.Email, dto.Name);
        if (!created) return StatusCode(500, "No s'ha pogut crear l'usuari");

        return Ok();
    }
}
```

Consideracions i bones pràctiques

- Comprovació d'existència: abans de crear l'usuari, comprova si ja existeix (p.ex. fent servir `IExistUserService`) per evitar duplicates i errors d'índex únic.
- Validacions: valida format d'email i longitud del nom abans de crear l'entitat de domini.
- Canonització: normalitza l'email (`Trim().ToLowerInvariant()`) si la política ho requereix.
- Errors i reintents: tracta errors de la BD i considera reintents o rollback segons la naturalesa de l'operació.
- Transaccionalitat: si la creació implicarà altres entitats relacionades, fes servir transaccions o mecanismes atomics si la BD les suporta.

Tests suggerits

1. Creació correcta: mock del repositori perquè `AddAsync` retorni `true`, assert que el servei retorna `true`.
2. Fallada al guardar: mock perquè `AddAsync` retorni `false` i verificar que el servei retorna `false`.
3. Error del repositori: mock que llança una excepció i verificar que es propaga.
4. Validacions prèvies: testar que el controller/servici de més alt nivell rebutja emails invàlids abans de cridar el servei.

Millores opcionals

- Integrar `IExistUserService` dins de `CreateUserService` per fer la comprovació d'existència abans de crear (pot evitar errors d'índex), però cal tenir en compte condicions de carrera i preferir una operació atòmica si la BD ho permet.
- Afegir logging per successos de creació i errors.
- Afegir un objecte de resposta ric amb informació de l'usuari creat o motiu de fallada.

Notes finals
La implementació actual és minimalista i delega la persistència al `IUsersCommandsRepository`. La validació, les comprovacions d'existència i la gestió d'errors s'han d'orquestrar des de nivells superiors (controller o serveis d'aplicació) segons la política del projecte.
