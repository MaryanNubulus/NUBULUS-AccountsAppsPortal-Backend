# ExistUser — Comprovar si un usuari existeix

Aquest fitxer descriu la feature `ExistUser`, que permet comprovar si ja existeix un usuari amb un correu electrònic determinat.

Fitxers rellevants

- `IExistUserService.cs` — interfície pública amb la signatura del servei.
- `ExistUserService.cs` — implementació que utilitza `IUsersQueriesRepository`.

Contracte

- Mètode públic: `Task<bool> ExistUserAsync(string email)`
  - Input: `email` (string) — el correu a comprovar. No es valida el format internament.
  - Output: `Task<bool>` — `true` si existeix, `false` en cas contrari.
  - Errors: es propaguen excepcions del repositori (errors de BD, timeouts, etc.).

Com funciona

1. `ExistUserService` rep via constructor una instància de `IUsersQueriesRepository`.
2. Executa la consulta sobre `GetAll()` i utilitza `AnyAsync(x => x.Email == email)` per determinar si hi ha coincidències.

Exemple d'injecció DI (Startup/Program)

```csharp
// Exemple de registre de serveis (pseudocodi)
services.AddScoped<IUsersQueriesRepository, UsersQueriesRepository>();
services.AddScoped<IExistUserService, ExistUserService>();
```

Ús en codi

```csharp
public class UsersController : ControllerBase
{
    private readonly IExistUserService _existUserService;

    public UsersController(IExistUserService existUserService)
    {
        _existUserService = existUserService;
    }

    public async Task<IActionResult> Register(UserDto dto)
    {
        if (await _existUserService.ExistUserAsync(dto.Email))
            return Conflict("El email ja està registrat");

        // continuar amb creació d'usuari
    }
}
```

Consideracions

- Validació d'entrada: el servei no valida format ni nul·litat. Valida l'email abans de cridar la funció.
- Canonització: si la BD no és case-sensitive o els emails es guarden normalitzats, normalitza (`Trim().ToLowerInvariant()`) abans de consultar.
- Índex: assegura un índex sobre el camp `Email` per fer la consulta eficient.
- Condicions de carrera: si després de comprovar existeix s'intenta inserir, utilitza operacions atòmiques (upsert/transaction) per evitar duplicates.
- Errors de BD: gestiona reintents o captura d'excepcions en nivells superiors si cal.

Tests suggerits

1. Usuari existent: mock del repositori perquè AnyAsync retorni `true` i assert que el servei retorna `true`.
2. Usuari no existent: mock que retorni `false` i assert per `false`.
3. Null/empty: definir la política (excepció o `false`) i crear tests per a aquest comportament.
4. Error de repositori: mock que llança excepció i verificar que es propaga.

Millores opcionals

- Afegir normalització d'email dins del servei.
- Afegir logs per a consultes fallides.
- Afegir una variant que exclogui un `userId` (útil en updates de perfil).

Notes finals
La implementació actual és simple i correcta per a la majoria de casos d'ús. Les decisions sobre validació, normalització i manipulació d'errors s'han d'establir a nivell d'arquitectura del projecte i aplicar de forma coherent als altres serveis d'usuaris.
