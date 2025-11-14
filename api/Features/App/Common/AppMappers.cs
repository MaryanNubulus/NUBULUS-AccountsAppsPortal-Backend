using Nubulus.Domain.Entities.App;

namespace Nubulus.Backend.Api.Features.App.Common;

public static class AppMappers
{
    public static AppDto ToDto(this AppEntity entity)
    {
        return new AppDto
        {
            Id = entity.AppId.Value,
            Key = entity.AppKey.Value,
            Name = entity.Name,
            Status = entity.Status.Value
        };
    }

    public static List<AppDto> ToDto(this List<AppEntity> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}
