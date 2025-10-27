using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee CreateEntity(CreateEmployeeRequest request)
        {
            return new Employee
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                IsActive = true
            };
        }

        public static EmployeeInfoDTO ToDTO(Employee employee)
        {
            return new EmployeeInfoDTO
            {
                Id = employee.Id,
                Email = employee.Email,
                Name = employee.Name,
                IsActive = employee.IsActive
            };
        }
    }
}
