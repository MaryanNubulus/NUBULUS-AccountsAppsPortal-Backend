using Nubulus.Domain.Abstractions;

namespace Nubulus.Domain.Entities.Account;

public record UpdateAccountDomainEvent() : IDomainEvent
{
    public string Id { get; } = Guid.NewGuid().ToString();

    public string EventName { get; } = "UpdateAccountDomainEvent";

    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}