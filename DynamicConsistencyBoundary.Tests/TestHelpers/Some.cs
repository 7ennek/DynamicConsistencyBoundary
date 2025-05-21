using DynamicConsistencyBoundary.Tests.Framework;

namespace DynamicConsistencyBoundary.Tests.TestHelpers;

public static partial class Some
{
    public static Guid Guid => Guid.NewGuid();
    public static string String => Some.Guid.ToString();
}

public static partial class Some
{
    public static EventType EventType => EventType.For(Some.String);
    public static object EventData => new { Field = Some.String };
    public static DomainConcept DomainConcept => new (Some.String);
    public static DomainInstanceId DomainInstanceId => new (Some.Guid);
    public static DomainIdentifier DomainIdentifier => DomainIdentifier.For(DomainInstanceId, Some.DomainConcept);
    public static DomainEvent Event => DomainEvent.Define(Some.EventType, Some.EventData);
}