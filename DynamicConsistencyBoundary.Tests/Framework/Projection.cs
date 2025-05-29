namespace DynamicConsistencyBoundary.Tests.Framework;

public interface IProjection<TState, TCondition>
    where TCondition : ICondition
{
    TCondition Condition { get; }
    (TState State, TCondition Condition) Apply(DomainEvent[] domainEvents);
}

public interface ICondition
{
    ISpecification<DomainEvent> On { get; }
}