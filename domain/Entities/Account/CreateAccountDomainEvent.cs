using Nubulus.Domain.Abstractions;

namespace Nubulus.Domain.Entities.Account;

public record CreateAccountDomainEvent() : IDomainEvent
{
    public string Id { get; } = Guid.NewGuid().ToString();

    public string EventName { get; } = "CreateAccountDomainEvent";

    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}