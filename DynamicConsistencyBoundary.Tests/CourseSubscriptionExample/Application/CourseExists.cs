using DynamicConsistencyBoundary.Tests.Framework;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;

public class CourseExistsCondition(Guid courseId) : ICondition
{
    public static CourseExistsCondition For(Guid courseId) => new(courseId);
    
    public ISpecification<DomainEvent> On => 
        DomainIdentifierSpecification.For(courseId, "Course")
        &
        EventTypeSpecification.For<CourseDefined>()
        ;
}

public record CourseExists(Guid CourseId, bool Exists);
public class CourseExistsProjection(Guid courseId, CourseExistsCondition condition) : IProjection<CourseExists, CourseExistsCondition>
{
    public static CourseExistsProjection For(Guid courseId) => new (courseId, CourseExistsCondition.For(courseId));
    public CourseExistsCondition Condition => condition;
    public (CourseExists State, CourseExistsCondition Condition) Apply(DomainEvent[] events)
    {
        var result = new CourseExists(courseId, false);
        foreach (var @event in events)
            switch (@event.Data)
            {
                case CourseDefined defined:
                    result = result with { Exists = true };
                    break;
                default:
                    break;
            }
        return (result, condition);
    }
}