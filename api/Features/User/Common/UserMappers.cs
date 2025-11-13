using Nubulus.Domain.Entities.User;

namespace Nubulus.Backend.Api.Features.User.Common;

public static class UserMappers
{
    public static UserDto ToDto(this UserEntity entity)
    {
        return new UserDto
        {
            UserId = entity.UserId.Value,
            UserKey = entity.UserKey.Value,
            FullName = entity.FullName,
            Email = entity.Email.Value,
            Phone = entity.Phone,
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
            UserKey = entity.UserKey.Value,
            FullName = entity.FullName,
            Email = entity.Email.Value,
            Phone = entity.Phone,
            Status = entity.Status.Value,
            IsCreator = entity.IsCreator
        };
    }

    public static UserToShareDto ToShareDto(this UserEntity entity)
    {
        return new UserToShareDto
        {
            UserId = entity.UserId.Value,
            FullName = entity.FullName,
            Email = entity.Email.Value
        };
    }

    public static List<UserToShareDto> ToShareDto(this IEnumerable<UserEntity> entities)
    {
        return entities.Select(e => e.ToShareDto()).ToList();
    }
}
