# Mòdul Employees

Aquest mòdul gestiona les operacions relacionades amb els empleats dins l'aplicació. Proporciona serveis per crear, obtenir i consultar informació d'empleats, així com endpoints REST per accedir-hi.

---

## Objectius

- Gestionar la creació i consulta d'empleats.
- Proporcionar endpoints REST per accedir a la informació d'empleats.
- Integrar amb el sistema d'autenticació per identificar l'empleat actual.

---

## Endpoints Principals

| Endpoint                    | Mètode | Descripció                                 |
| --------------------------- | ------ | ------------------------------------------ |
| `/api/v1/employees`         | GET    | Retorna la llista d'empleats.              |
| `/api/v1/employees/current` | GET    | Retorna la informació de l'empleat actual. |

---

## Requisits

- **.NET 8** o superior.
- **ASP.NET Core Minimal APIs**.
- Serveis interns:
  - `ICreateEmployeeService`
  - `IGetEmployeesService`
  - `IGetCurrentEmployeeService`

---

## Configuració

1. Afegir els endpoints al pipeline:
   ```csharp
   app.MapGetEmployeesEndPoint();
   app.MapGetCurrentEmployeeEndPoint();
   ```
2. Configurar serveis:
   ```csharp
   builder.Services.AddEmployeesServices();
   ```

---

## Flux de Funcionament

1. L'usuari autenticat accedeix a `/employees/current` per obtenir la seva informació.
2. L'administrador pot consultar tots els empleats via `/employees`.
3. La creació d'empleats es fa internament via `CreateEmployeeService`.

---

## Notes

- L'endpoint per crear empleats no està exposat públicament.
- Tots els endpoints requereixen autorització.
- El mòdul segueix les millors pràctiques de DI i separació de responsabilitats.
