using DynamicConsistencyBoundary.Tests.Framework;
using DynamicConsistencyBoundary.Tests.TestHelpers;
using Shouldly;

namespace DynamicConsistencyBoundary.Tests.FrameworkTests;

public class StoreQueryingTests
{
    private readonly InMemoryEventStore _eventStore = new();
    
    [Fact]
    public void be_able_to_query_based_on_event_type()
    {
        _eventStore.Append(new DummyEvent());
        _eventStore.Append(Some.EventData);
        _eventStore.Query(EventTypeSpecification.For<DummyEvent>()).Result.ShouldHaveSingleItem();
    }

    [Fact]
    public void be_able_to_query_based_on_identifiers()
    {
        var identifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [identifier]);
        _eventStore.Append(Some.EventData, [identifier, Some.DomainIdentifier]);
        _eventStore.Query(DomainIdentifierSpecification.For(identifier)).Result.Count().ShouldBe(2);
    }
    
    [Fact]
    public void be_able_to_query_based_on_identifiers_not()
    {
        var identifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [identifier]);
        _eventStore.Append(Some.EventData, [identifier, Some.DomainIdentifier]);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        _eventStore.Query(!DomainIdentifierSpecification.For(identifier)).Result.Count().ShouldBe(1);
    }

    [Fact]
    public void be_able_to_query_based_on_multiple_identifiers_or()
    {
        var firstIdentifier = Some.DomainIdentifier;
        var secondIdentifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [firstIdentifier]);
        _eventStore.Append(Some.EventData, [firstIdentifier, secondIdentifier]);
        _eventStore.Append(Some.EventData, [secondIdentifier]);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        _eventStore.Query(
            DomainIdentifierSpecification.For(firstIdentifier) 
            | DomainIdentifierSpecification.For(secondIdentifier)
        ).Result.Count().ShouldBe(3);
    }
    
    [Fact]
    public void be_able_to_query_based_on_multiple_identifiers_or_not()
    {
        var firstIdentifier = Some.DomainIdentifier;
        var secondIdentifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [firstIdentifier]);
        _eventStore.Append(Some.EventData, [firstIdentifier, secondIdentifier]);
        _eventStore.Append(Some.EventData, [secondIdentifier]);
        _eventStore.Query(
            !(DomainIdentifierSpecification.For(firstIdentifier) 
              | DomainIdentifierSpecification.For(secondIdentifier))
        ).Result.Count().ShouldBe(0);
    }
    
    [Fact]
    public void be_able_to_query_based_on_multiple_identifiers_and()
    {
        var firstIdentifier = Some.DomainIdentifier;
        var secondIdentifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [firstIdentifier]);
        _eventStore.Append(Some.EventData, [firstIdentifier, secondIdentifier]);
        _eventStore.Append(Some.EventData, [secondIdentifier]);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        _eventStore.Query(
            DomainIdentifierSpecification.For(firstIdentifier) 
            & DomainIdentifierSpecification.For(secondIdentifier)
        ).Result.Count().ShouldBe(1);
    }
    
    [Fact]
    public void be_able_to_query_based_on_multiple_identifiers_and_not()
    {
        var firstIdentifier = Some.DomainIdentifier;
        var secondIdentifier = Some.DomainIdentifier;
        _eventStore.Append(Some.EventData, [firstIdentifier]);
        _eventStore.Append(Some.EventData, [firstIdentifier, secondIdentifier]);
        _eventStore.Append(Some.EventData, [secondIdentifier]);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        _eventStore.Query(
            !(DomainIdentifierSpecification.For(firstIdentifier) 
              & DomainIdentifierSpecification.For(secondIdentifier))
        ).Result.Count().ShouldBe(3);
    }
    
    [Fact]
    public void be_able_to_query_based_on_domain_concept()
    {
        var domainConcept = Some.DomainConcept;
        _eventStore.Append(Some.EventData, [DomainIdentifier.For(Some.DomainInstanceId, domainConcept)]);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        _eventStore.Query(DomainConceptSpecification.For(domainConcept)).Result.ShouldHaveSingleItem();
    }

    [Fact]
    public void be_able_to_query_based_on_domain_instance_id()
    {
        var domainInstanceId = Some.DomainInstanceId;
        _eventStore.Append(Some.EventData, [DomainIdentifier.For(domainInstanceId, Some.DomainConcept)]);
        _eventStore.Append(Some.EventData, [Some.DomainIdentifier]);
        _eventStore.Query(DomainInstanceIdSpecification.For(domainInstanceId)).Result.ShouldHaveSingleItem();
    }
    
    private record DummyEvent();
}