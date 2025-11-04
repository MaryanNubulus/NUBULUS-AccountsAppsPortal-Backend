# Mòdul Accounts

Aquest mòdul s’encarrega de gestionar les operacions associades als comptes del sistema. Inclou serveis per crear, consultar, actualitzar i administrar l’estat d’habilitació dels comptes, així com els endpoints REST corresponents.

---

## Objectius

- Gestionar la creació, consulta i actualització de comptes.
- Permetre deshabilitar i habilitar comptes.
- Al crear un compte es crea un usuari propietari del compte.
- Proporcionar endpoints REST per accedir a la informació de comptes.

---

## Endpoints Principals

| Endpoint                                  | Mètode | Descripció                                     |
| ----------------------------------------- | ------ | ---------------------------------------------- |
| `/api/v1/accounts`                        | POST   | Crea un nou compte i un nou usuari propietari. |
| `/api/v1/accounts`                        | GET    | Retorna la llista de comptes.                  |
| `/api/v1/accounts/{accountId}/deactivate` | POST   | Desactivar un compte.                          |
| `/api/v1/accounts/{accountId}/activate`   | POST   | Activar un compte.                             |
| `/api/v1/accounts/{accountId}`            | PUT    | Actualitza la informació d'un compte           |

---

## Requisits

- **.NET 8** o superior.
- **ASP.NET Core Minimal APIs**.
- Serveis interns de comptes:

  - `ICreateAccountService`
  - `IGetAccountsService`
  - `IDesactiveActivateAccountService`
  - `IUpdateAccountService`

---

## Configuració

1. Afegir els endpoints al pipeline:
   ```csharp
   app.MapAppsEndpoints();
   ```
2. Configurar serveis:
   ```csharp
   builder.Services.AddAppsServices();
   ```

---

## Flux de Funcionament

1. L'empleat autenticat pot crear un compte via `POST /accounts`.
2. Al crear un compte tambe es crea un usuari propietari del compte.
3. Pot consultar tots els comptes via `GET /accounts`.
4. Pot desactivar o activar un compte amb `POST /accounts/{id}/desactivate` o `activate`.
5. Pot actualitzar la informació d'una aplicació amb `PUT /accounts/{id}`.

---

## Notes

- Tots els endpoints requereixen autorització.
- Les operacions inclouen validació i gestió d'errors detallada.
- Els comptes poden estar deshabilitats o habilitats sense perdre la informació.
- El mòdul segueix els principis de Clean Architecture.
