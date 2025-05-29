using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;

namespace DynamicConsistencyBoundary.Tests.Framework;

public class InMemoryEventStore
{
    private long _position = 0;
    private readonly List<DomainEvent> _events = [];
    private readonly Lock _lock = new ();
    
    public void Append(EventType eventType, object eventData) => Append(eventType, eventData, []);
    public void Append(EventType eventType, object eventData, long? lastKnownPosition) => Append(eventType, eventData, [], lastKnownPosition);
    public void Append(EventType eventType, object eventData, DomainIdentifier[] identifiers) => Append(eventType, eventData, identifiers, (long?) null);
    public void Append(EventType eventType, object eventData, DomainIdentifier[] identifiers, ISpecification<DomainEvent> query) => Append(eventType, eventData, identifiers, -1, query);
    public void Append(
        EventType eventType,
        object eventData,
        DomainIdentifier[] identifiers,
        long lastKnownPosition, 
        ISpecification<DomainEvent> query
    )
    {
        lock (_lock)
        {
            var (_, lastFoundPosition) = Query(query);
            if (lastKnownPosition != lastFoundPosition)
                throw new InvalidOperationException($"unexpected position: {lastKnownPosition}");
            AppendEventToStore(eventType, eventData, identifiers);
        }
    }
    
    private void Append(EventType eventType, object eventData, DomainIdentifier[] identifiers, long? lastKnownPosition)
    {
        lock (_lock)
        {
            if (lastKnownPosition is not null && lastKnownPosition.Value != _position - 1)
                throw new InvalidOperationException($"unexpected position: {lastKnownPosition}");

            AppendEventToStore(eventType, eventData, identifiers);
        }
    }
    
    private void AppendEventToStore(EventType eventType, object eventData, DomainIdentifier[] identifiers)
    {
        _events.Add(DomainEvent.Define(
            eventType,
            _position,
            eventData,
            identifiers));
        _position++;
    }
    
    public (IEnumerable<DomainEvent> Result, long LastKnownPosition) Query()
    {
        lock (_lock)
        {
            var result = _events.AsReadOnly();
            return (result, result.LastOrDefault()?.Position ?? -1);
        }
    }

    public (IEnumerable<DomainEvent> Result, long LastKnownPosition) Query(ISpecification<DomainEvent> specification)
    {
        lock (_lock)
        {
            var result = _events.Where(specification.IsSatisfiedBy).ToArray();
            return (result, result.LastOrDefault()?.Position ?? -1);
        }
    }

    public (TState State, ICondition Condition)  Project<TState, TCondition>(IProjection<TState, TCondition> projection)
        where TCondition : ICondition
    {
        var (events, _) = Query(projection.Condition.On);
        return projection.Apply(events.ToArray());
    }
}