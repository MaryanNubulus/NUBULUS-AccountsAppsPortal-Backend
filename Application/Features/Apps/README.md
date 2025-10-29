# Mòdul Apps

Aquest mòdul gestiona les operacions relacionades amb les aplicacions dins el sistema. Proporciona serveis per crear, obtenir, actualitzar i gestionar l'estat d'activació de les aplicacions, així com endpoints REST per accedir-hi.

---

## Objectius

- Gestionar la creació, consulta i actualització d'aplicacions.
- Permetre pausar i reprendre aplicacions.
- Proporcionar endpoints REST per accedir a la informació d'aplicacions.

---

## Endpoints Principals

| Endpoint                      | Mètode | Descripció                                |
| ----------------------------- | ------ | ----------------------------------------- |
| `/api/v1/apps`                | POST   | Crea una nova aplicació.                  |
| `/api/v1/apps`                | GET    | Retorna la llista d'aplicacions.          |
| `/api/v1/apps/{appId}/pause`  | POST   | Pausa una aplicació específica.           |
| `/api/v1/apps/{appId}/resume` | POST   | Reprèn una aplicació pausada.             |
| `/api/v1/apps/{id}`           | PUT    | Actualitza la informació d'una aplicació. |

---

## Requisits

- **.NET 8** o superior.
- **ASP.NET Core Minimal APIs**.
- Serveis interns:
  - `ICreateAppService`
  - `IGetAppsService`
  - `IPauseResumeAppService`
  - `IUpdateAppService`

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

1. L'empleat autenticat pot crear una aplicació via `POST /apps`.
2. Pot consultar totes les aplicacions via `GET /apps`.
3. Pot pausar o reprendre una aplicació amb `POST /apps/{id}/pause` o `resume`.
4. Pot actualitzar la informació d'una aplicació amb `PUT /apps/{id}`.

---

## Notes

- Tots els endpoints requereixen autorització.
- Les operacions inclouen validació i gestió d'errors detallada.
- Les aplicacions poden ser pausades i represes sense perdre la configuració.
- El mòdul segueix els principis de Clean Architecture.
