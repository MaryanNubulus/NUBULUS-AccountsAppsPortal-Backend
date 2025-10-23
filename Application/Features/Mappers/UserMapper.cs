using NUBULUS.AccountsAppsPortalBackEnd.Application.Features.DTOs;
using NUBULUS.AccountsAppsPortalBackEnd.Domain.Entities;

namespace NUBULUS.AccountsAppsPortalBackEnd.Application.Features.Mappers
{
    public static class UserMapper
    {
        public static User CreateEntity(CreateUserRequest request)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                IsActive = true
            };
        }

        public static UserInfoDTO ToDTO(User user)
        {
            return new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                IsActive = user.IsActive
            };
        }
    }
}
