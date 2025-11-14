using Nubulus.Domain.ValueObjects;

namespace Nubulus.Domain.Entities.App;

public class AppEntity
{
    public AppId AppId { get; set; } = default!;
    public AppKey AppKey { get; set; } = default!;
    public string Name { get; set; } = default!;
    public Status Status { get; set; } = Status.Active;
}
