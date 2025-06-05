namespace DynamicConsistencyBoundary.Tests.Framework;

public interface ISpecification<in T>  
{  
    bool IsSatisfiedBy(T item);
    
    public static AndSpecification<T> operator &(ISpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static OrSpecification<T> operator |(ISpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static NotSpecification<T> operator !(ISpecification<T> spec) => new (spec);
}

public class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right) : ISpecification<T>
{
    public bool IsSatisfiedBy(T item) => left.IsSatisfiedBy(item) && right.IsSatisfiedBy(item);
    public static AndSpecification<T> For(ISpecification<T> left, ISpecification<T> right) => new (left, right);
    
    public static AndSpecification<T> operator &(AndSpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static OrSpecification<T> operator |(AndSpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static NotSpecification<T> operator !(AndSpecification<T> spec) => new (spec);
}

public class OrSpecification<T>(ISpecification<T> left, ISpecification<T> right) : ISpecification<T>
{
    public bool IsSatisfiedBy(T item) => left.IsSatisfiedBy(item) || right.IsSatisfiedBy(item);
    public static OrSpecification<T> For(ISpecification<T> left, ISpecification<T> right) => new (left, right);
    
    public static AndSpecification<T> operator &(OrSpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static OrSpecification<T> operator |(OrSpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static NotSpecification<T> operator !(OrSpecification<T> spec) => new (spec);
}

public class NotSpecification<T>(ISpecification<T> specification) : ISpecification<T>
{
    public bool IsSatisfiedBy(T item) => !specification.IsSatisfiedBy(item);
    public static NotSpecification<T> For(ISpecification<T> instance) => new (instance);
    
    public static AndSpecification<T> operator &(NotSpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static OrSpecification<T> operator |(NotSpecification<T> spec1, ISpecification<T> spec2) => new (spec1, spec2);
    public static NotSpecification<T> operator !(NotSpecification<T> spec) => new (spec);
}

public class DomainIdentifierSpecification(DomainIdentifier identifier) : ISpecification<DomainEvent>
{
    public bool IsSatisfiedBy(DomainEvent item) => item.Identifiers.Contains(identifier);

    public static DomainIdentifierSpecification For(Guid id, string concept) => For(DomainIdentifier.For(id, concept));
    public static DomainIdentifierSpecification For(DomainIdentifier identifier) => new(identifier);
    
    public static AndSpecification<DomainEvent> operator &(DomainIdentifierSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static OrSpecification<DomainEvent> operator |(DomainIdentifierSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static NotSpecification<DomainEvent> operator !(DomainIdentifierSpecification spec) => new (spec);
}

public class DomainConceptSpecification(DomainConcept concept) : ISpecification<DomainEvent>
{
    public bool IsSatisfiedBy(DomainEvent item) => item.Identifiers.Any(x => x.Concept == concept);

    public static DomainConceptSpecification For(DomainConcept concept) => new(concept);
    
    public static AndSpecification<DomainEvent> operator &(DomainConceptSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static OrSpecification<DomainEvent> operator |(DomainConceptSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static NotSpecification<DomainEvent> operator !(DomainConceptSpecification spec) => new (spec);
}

public class DomainInstanceIdSpecification(DomainInstanceId domainInstanceId) : ISpecification<DomainEvent>
{
    public bool IsSatisfiedBy(DomainEvent item) => item.Identifiers.Any(x => x.InstanceId == domainInstanceId);
    public static DomainInstanceIdSpecification For(DomainInstanceId domainInstanceId) => new(domainInstanceId);
    
    public static AndSpecification<DomainEvent> operator &(DomainInstanceIdSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static OrSpecification<DomainEvent> operator |(DomainInstanceIdSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static NotSpecification<DomainEvent> operator !(DomainInstanceIdSpecification spec) => new (spec);
}

public class EventTypeSpecification(EventType eventType) : ISpecification<DomainEvent>
{
    public bool IsSatisfiedBy(DomainEvent item) => item.Type == eventType;
    public static EventTypeSpecification For(EventType eventType) => new(eventType);
    public static EventTypeSpecification For<T>() => new(EventType.For(typeof(T).Name));
    
    public static AndSpecification<DomainEvent> operator &(EventTypeSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static OrSpecification<DomainEvent> operator |(EventTypeSpecification spec1, ISpecification<DomainEvent> spec2) => new (spec1, spec2);
    public static NotSpecification<DomainEvent> operator !(EventTypeSpecification spec) => new (spec); 
}