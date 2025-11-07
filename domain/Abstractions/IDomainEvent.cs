namespace Nubulus.Domain.Abstractions;

public interface IDomainEvent
{
    string Id { get; }

    string EventName { get; }

    DateTime OccurredOn { get; }
}