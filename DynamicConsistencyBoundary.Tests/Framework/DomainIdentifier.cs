namespace DynamicConsistencyBoundary.Tests.Framework;

public record DomainIdentifier(DomainInstanceId InstanceId, DomainConcept Concept)
{
    public static DomainIdentifier For(Guid entityId, string concept) =>
        For(DomainInstanceId.For(entityId), DomainConcept.For(concept));
    
    public static DomainIdentifier For(DomainInstanceId entityId, DomainConcept concept)
    {
        if (entityId is null || entityId.Value == Guid.Empty) throw new ArgumentException($"{nameof(entityId)} cannot be empty", nameof(entityId));
        if (string.IsNullOrEmpty(concept?.Value))  throw new ArgumentException($"{nameof(concept)} cannot be empty", nameof(concept));
        return new DomainIdentifier(entityId, concept);
    }
}

public record DomainConcept(string Value)
{
    public static DomainConcept For(string concept) => new(concept);
}

public record DomainInstanceId(Guid Value)
{
    public static DomainInstanceId For(Guid domainInstanceId) => new(domainInstanceId);
}