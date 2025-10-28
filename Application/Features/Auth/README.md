# Mòdul d'Autenticació - AccountsAppsPortalBackEnd

## **Definició del Mòdul**
Aquest mòdul implementa els **endpoints d'autenticació i gestió de sessió** per a l'aplicació **AccountsAppsPortalBackEnd**. Està dissenyat per treballar amb **Microsoft Authentication** i integra serveis interns per validar i crear usuaris dins del sistema.

Inclou:
- **Sign-In**: Autenticació via OpenID Connect i creació d'empleats si no existeixen.
- **Sign-Out**: Tancament de sessió i redirecció.
- **Validació de Sessió**: Comprovació si l'usuari està autenticat.

---

## **Objectius**
- Permetre l'autenticació segura d'usuaris mitjançant **OpenID Connect**.
- Gestionar la persistència d'empleats (crear si no existeixen).
- Proporcionar endpoints REST minimalistes per a integració amb el **frontend**.
- Garantir compatibilitat amb **Microsoft Identity Platform**.

---

## **Endpoints Principals**
| Endpoint | Mètode | Descripció |
|----------|--------|------------|
| `/api/v1/auth/sign-in` | GET | Inicia el procés d'autenticació i redirigeix a `/success`. |
| `/api/v1/auth/success` | GET | Valida l'usuari autenticat, crea l'empleat si no existeix, i redirigeix al frontend privat. |
| `/api/v1/auth/sign-out` | GET | Tanca la sessió i redirigeix a la URL indicada (o per defecte `http://localhost:5173/`). |
| `/api/v1/auth/is-valid-session` | GET | Retorna `200 OK` si la sessió és vàlida, `401 Unauthorized` si no. |

---

## **Requisits**
- **.NET 8** o superior.
- **ASP.NET Core Minimal APIs**.
- **Microsoft.AspNetCore.Authentication.OpenIdConnect**.
- **Microsoft.AspNetCore.Authentication.Cookies**.
- Serveis interns:
  - `IExistEmployeeService`
  - `ICreateEmployeeService`
- **Configuració prèvia d'OpenID Connect** amb Microsoft.

---

## **Configuració**
1. Afegir els endpoints al pipeline:
   ```csharp
   app.MapSignInEndpoint();
   app.MapSignOutEndpoint();
   app.MapIsValidSessionEndpoint();
   ```
2. Configurar autenticació:
   ```csharp
   builder.Services.AddAuthentication(options => { ... })
       .AddCookie()
       .AddOpenIdConnect(options => { ... });
   ```
3. Definir els serveis `IExistEmployeeService` i `ICreateEmployeeService` al DI container.

---

## **Flux d'Autenticació**
1. L'usuari accedeix a `/sign-in`.
2. Es redirigeix a Microsoft per autenticar-se.
3. En tornar a `/success`:
   - Es valida si l'empleat existeix.
   - Si no, es crea amb les dades del token.
4. Es redirigeix al frontend privat.

---

## **Notes**
- El mòdul assumeix que el **frontend** està a `http://localhost:5173/`.
- Els endpoints són **minimalistes** i segueixen el patró REST.
- Es pot adaptar fàcilment per entorns de producció canviant les URLs.
