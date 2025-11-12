using Nubulus.Domain.Entities.User;

namespace Nubulus.Backend.Api.Features.User.Common;

public static class UserMappers
{
    public static UserDto ToDto(this UserEntity entity)
    {
        return new UserDto
        {
            UserId = entity.UserId.Value,
            Name = entity.Name,
            Email = entity.Email.Value,
            Status = entity.Status.Value,
            IsCreator = entity.IsCreator
        };
    }

    public static List<UserDto> ToDto(this IEnumerable<UserEntity> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static UserInfoDto ToInfoDto(this UserEntity entity)
    {
        return new UserInfoDto
        {
            UserId = entity.UserId.Value,
            Name = entity.Name,
            Email = entity.Email.Value,
            Status = entity.Status.Value,
            IsCreator = entity.IsCreator
        };
    }
}
