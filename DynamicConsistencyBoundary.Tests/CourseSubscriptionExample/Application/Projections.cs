using DynamicConsistencyBoundary.Tests.Framework;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;

public interface IProjection<TState>
{
    ISpecification<DomainEvent> On();
    (TState State, ISpecification<DomainEvent> Condition) Apply(DomainEvent[] domainEvents);
}

public record CourseExists(bool Exists);

public class CourseExistsProjection(Guid courseId) : IProjection<CourseExists>
{
    public static CourseExistsProjection For(Guid courseId) => new (courseId);

    public ISpecification<DomainEvent> On()
    {
        return DomainIdentifierSpecification.For(DomainIdentifier.For(courseId, "Course"));
    }

    public (CourseExists State, ISpecification<DomainEvent> Condition) Apply(DomainEvent[] events)
    {
        var result = new CourseExists(false);
        foreach (var @event in events)
            switch (@event.Data)
            {
                case CourseDefined defined:
                    result = new CourseExists(Exists: true);
                    break;
                default:
                    break;
            }
        return (result, On());
    }
}