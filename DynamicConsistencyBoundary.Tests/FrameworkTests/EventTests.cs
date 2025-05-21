using DynamicConsistencyBoundary.Tests.Framework;
using DynamicConsistencyBoundary.Tests.TestHelpers;
using Shouldly;

namespace DynamicConsistencyBoundary.Tests.FrameworkTests;

public class EventTests
{
    [Fact]
    public void define_an_event()
    {
        var eventData = Some.EventData;
        var eventType = Some.EventType;
        
        var @event = DomainEvent.Define(eventType, eventData);
        @event.Id.ShouldNotBe(Guid.Empty);
        @event.Data.ShouldBe(eventData);
        @event.Type.ShouldBe(eventType);
        @event.Timestamp.ShouldNotBe(DateTimeOffset.MinValue);
        @event.Identifiers.ShouldBeEmpty();
    }
    
    [Fact]
    public void define_an_event_with_identifiers()
    {
        var entityId = Some.DomainInstanceId;
        var eventData = Some.EventData;
        var eventType = Some.EventType;
        var identifier = DomainIdentifier.For(entityId, Some.DomainConcept);
        
        var @event = DomainEvent.Define(eventType, eventData, [identifier]);
        @event.Id.ShouldNotBe(Guid.Empty);
        @event.Data.ShouldBe(eventData);
        @event.Type.ShouldBe(eventType);
        @event.Timestamp.ShouldNotBe(DateTimeOffset.MinValue);
        @event.Identifiers.ShouldHaveSingleItem();
        @event.Identifiers.Single().ShouldBe(identifier);
    }
}