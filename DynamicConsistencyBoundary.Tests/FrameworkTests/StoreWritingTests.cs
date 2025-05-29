using DynamicConsistencyBoundary.Tests.Framework;
using DynamicConsistencyBoundary.Tests.TestHelpers;
using Shouldly;

namespace DynamicConsistencyBoundary.Tests.FrameworkTests;

public class StoreWritingTests
{
    private readonly InMemoryEventStore _eventStore = new();

    [Fact]
    public void store_events()
    {
        _eventStore.Append(Some.EventData);
        _eventStore.Query().Result.ShouldHaveSingleItem();
    }
    
    [Fact]
    public void storing_an_event_should_give_it_a_position()
    {
        _eventStore.Append(Some.EventData);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        var (_, lastKnownPosition) = _eventStore.Query();
        lastKnownPosition.ShouldBe(1);
    }

    [Fact]
    public void storing_an_event_with_expected_position_should_succeed()
    {
        _eventStore.Append(Some.EventData);
        _eventStore.Append(Some.EventData, 0);
        _eventStore.Query().Result.Count().ShouldBe(2);
    }
    
    [Fact]
    public void storing_an_event_with_unexpected_position_should_fail()
    {
        _eventStore.Append(Some.EventData);
        _eventStore.Append(Some.EventData);
        Should.Throw<InvalidOperationException>(() =>
        {
            _eventStore.Append(Some.EventData, 0);
        });
    }

    [Fact]
    public void storing_an_event_with_query()
    {
        var identifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [identifier]);
        _eventStore.Append(Some.EventData, [identifier, Some.DomainIdentifier]);
        _eventStore.Append(Some.EventData); //event that we don't care about
        _eventStore.Append(Some.EventData); //event that we don't care about
        
        var query = DomainIdentifierSpecification.For(identifier);
        var (_, lastKnownPosition) = _eventStore.Query(query);
        
        lastKnownPosition.ShouldBe(1);

        Should.NotThrow(() =>
        {
            _eventStore.Append(Some.EventData, [identifier], lastKnownPosition, query);
        });
    }
    
    [Fact]
    public void storing_an_event_with_query_while_another_event_is_appended_should_fail()
    {
        var identifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [identifier]);
        _eventStore.Append(Some.EventData, [identifier, Some.DomainIdentifier]);
        _eventStore.Append(Some.EventData); //event that we don't care about
        _eventStore.Append(Some.EventData); //event that we don't care about
        
        var query = DomainIdentifierSpecification.For(identifier);
        var (_, lastKnownPosition) = _eventStore.Query(query);
        
        //some event that we DO care about is meanwhile saved
        _eventStore.Append(Some.EventData, [identifier]);
        
        Should.Throw<InvalidOperationException>(() =>
        {
            _eventStore.Append(Some.EventData, [identifier], lastKnownPosition, query);
        });
    }
}