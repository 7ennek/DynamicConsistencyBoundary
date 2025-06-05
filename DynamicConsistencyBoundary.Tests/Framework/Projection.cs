namespace DynamicConsistencyBoundary.Tests.Framework;

public interface IProjection<TState, TCondition>
    where TCondition : ICondition
{
    TCondition Condition { get; }
    (TState State, TCondition Condition) Apply(DomainEvent[] domainEvents);
}

public interface IDecision<TState, TCondition> : IProjection<TState, TCondition>
    where TCondition : ICondition
{
    (bool IsSatisfied, object[] ToApply, DomainIdentifier DomainIdentifier) ApplyWhenSatisfiedWith(TState state);
}