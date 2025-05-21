namespace DynamicConsistencyBoundary.Tests.Framework;

public class DomainEvent
{
    public Guid Id { get; init; }
    public EventType Type { get; init; }
    public long Position { get; init; }
    public object Data { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public IReadOnlyList<DomainIdentifier> Identifiers { get; }
    
    private DomainEvent(Guid id, EventType type, long position, object data,  DateTimeOffset timestamp, IReadOnlyList<DomainIdentifier> identifiers)
    {
        Id = id;
        Data = data;
        Type = type;
        Position = position;
        Timestamp = timestamp;
        Identifiers = identifiers;
    }

    public static DomainEvent Define(EventType type, object data) => Define(type, data, []);
    public static DomainEvent Define(EventType type, object data, IReadOnlyList<DomainIdentifier> identifiers) => Define(type,0, data, identifiers);
    public static DomainEvent Define(EventType type, long position, object data, IReadOnlyList<DomainIdentifier> identifiers)
    {
        if (string.IsNullOrEmpty(type?.Value)) throw new ArgumentException($"{nameof(type)} cannot be empty", nameof(type));
        if (data is null) throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null");
        if (identifiers is null) throw new ArgumentNullException(nameof(identifiers), $"{nameof(identifiers)} cannot be null");
        return new DomainEvent(Guid.NewGuid(), type, position, data, DateTimeOffset.UtcNow, identifiers);
    }
}

public record EventType(string Value)
{
    public static EventType For(string value) => new(value);
}