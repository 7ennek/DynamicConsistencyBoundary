using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;

namespace DynamicConsistencyBoundary.Tests.Framework;

public class InMemoryEventStore
{
    private long _position = 0;
    private readonly List<DomainEvent> _events = [];
    private readonly Lock _lock = new ();
    
    public void Append<TEvent>(TEvent eventData) where TEvent : class
        => Append(eventData, []);
    
    public void Append<TEvent>(TEvent eventData, long lastKnownPosition) where TEvent : class
        => Append(eventData,[], lastKnownPosition);
    
    public void Append<TEvent>(TEvent eventData, DomainIdentifier[] identifiers) where TEvent : class
        => Append(eventData, identifiers, (long?) null);
    
    public void Append<TEvent>(TEvent eventData, DomainIdentifier[] identifiers, ICondition condition) where TEvent : class
        => Append(eventData, identifiers, -1, condition);
    
    public void Append<TEvent>(
        TEvent eventData,
        DomainIdentifier[] identifiers,
        long lastKnownPosition, 
        ICondition condition
    ) where  TEvent : class
        => Append(eventData, identifiers, lastKnownPosition, condition.On);
    
    public void Append<TEvent>(
        TEvent eventData,
        DomainIdentifier[] identifiers,
        long lastKnownPosition, 
        ISpecification<DomainEvent> query
    ) where  TEvent : class
    {
        lock (_lock)
        {
            var (_, lastFoundPosition) = Query(query);
            if (lastKnownPosition != lastFoundPosition)
                throw new InvalidOperationException($"unexpected position: {lastKnownPosition}");
            AppendEventToStore(eventData, identifiers);
        }
    }
    
    private void Append<TEvent>(TEvent eventData, DomainIdentifier[] identifiers, long? lastKnownPosition)
        where TEvent : class
    {
        lock (_lock)
        {
            if (lastKnownPosition is not null && lastKnownPosition.Value != _position - 1)
                throw new InvalidOperationException($"unexpected position: {lastKnownPosition}");

            AppendEventToStore(eventData, identifiers);
        }
    }
    
    private void AppendEventToStore<TEvent>(TEvent eventData, DomainIdentifier[] identifiers)
        where TEvent : class
    {
        _events.Add(DomainEvent.Define(
            EventType.For(typeof(TEvent).Name),
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