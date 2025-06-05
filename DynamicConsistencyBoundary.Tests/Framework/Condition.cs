namespace DynamicConsistencyBoundary.Tests.Framework;

public interface ICondition
{
    ISpecification<DomainEvent> On { get; }
}

public class ConditionWrapper(ICondition[] wrapFor) : ICondition
{
    public static ConditionWrapper For(ICondition[] wrapFor) => new(wrapFor);
    public ISpecification<DomainEvent> On => 
        wrapFor.Select(c => c.On).Aggregate((acc, add) => acc | add);
}