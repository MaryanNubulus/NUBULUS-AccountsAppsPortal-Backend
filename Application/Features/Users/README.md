# Mòdul Users

Aquest mòdul s’encarrega de gestionar les operacions associades als usuaris del sistema. Inclou serveis per crear, consultar, actualitzar, i administrar l’estat d’habilitació dels usuaris , així com els endpoints REST corresponents.

---

## Objectius

- Gestionar la creació, consulta i actualització d'usuaris.
- Permetre deshabilitar i habilitar usuaris.
- Al crear un usuari es crea un usuari dintre del compte associat amb el rol corresponent (no pot ser propietari).
- Proporcionar endpoints REST per accedir a la informació d'usuaris.

---

## Endpoints Principals

| Endpoint                                                 | Mètode | Descripció                                                                                     |
| -------------------------------------------------------- | ------ | ---------------------------------------------------------------------------------------------- |
| `/api/v1/accounts/{accountId}/users`                     | POST   | Crea un nou usuari dintre del compte associat amb el rol corresponent (no pot ser propietari). |
| `/api/v1/accounts/{accountId}/users`                     | GET    | Retorna la llista d'usuaris d'un compte.                                                       |
| `/api/v1/accounts/{accountId}/users/{userId}`            | PUT    | Actualitza la informació d'un usuari.                                                          |
| `/api/v1/accounts/{accountId}/users/{userId}/deactivate` | POST   | Desactivar un usuari.                                                                          |
| `/api/v1/accounts/{accountId}/users/{userId}/activate`   | POST   | Activar un usuari.                                                                             |

---

## Requisits

- **.NET 8** o superior.
- **ASP.NET Core Minimal APIs**.
- Serveis interns de usuaris:

  - `ICreateUserService`
  - `IGetUsersService`
  - `IDeactivateActivateUserService`
  - `IUpdateUserService`

---

## Configuració

1. Afegir els endpoints al pipeline:
   ```csharp
   app.MapUsersEndpoints();
   ```
2. Configurar serveis:
   ```csharp
   builder.Services.AddUsersServices();
   ```

---

## Flux de Funcionament

1. L'empleat autenticat pot crear un usuari via `POST /accounts/{accountId}/users`.
2. Al crear un usuari s'assigna automàticament al compte associat amb el rol corresponent (no pot ser propietari).
3. Pot consultar tots els usuaris del compte via `GET /accounts/{accountId}/users`.
4. Pot desactivar o activar un usuari amb `POST /accounts/{accountId}/users/{userId}/deactivate` o `activate`.
5. Pot actualitzar la informació d'un usuari amb `PUT /accounts/{accountId}/users/{userId}`.

---

## Notes

- Tots els endpoints requereixen autorització.
- Les operacions inclouen validació i gestió d'errors detallada.
- Els comptes poden estar deshabilitats o habilitats sense perdre la informació.
- El mòdul segueix els principis de Clean Architecture.
