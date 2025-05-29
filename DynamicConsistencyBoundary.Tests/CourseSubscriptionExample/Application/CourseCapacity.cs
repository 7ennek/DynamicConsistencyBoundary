using DynamicConsistencyBoundary.Tests.Framework;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;

public class CourseChangeCapacityCondition(Guid courseId) : ICondition
{
    public static CourseChangeCapacityCondition For(Guid courseId) => new(courseId);
    public ISpecification<DomainEvent> On =>
        DomainIdentifierSpecification.For(courseId, "Course")
        &
        (
            EventTypeSpecification.For<CourseDefined>()
            | EventTypeSpecification.For<CourseCapacityChanged>()
        );
}

public record CourseCapacity(Guid CourseId, int Capacity);

public class CourseCapacityProjection(Guid courseId, CourseChangeCapacityCondition condition)
    : IProjection<CourseCapacity, CourseChangeCapacityCondition>
{
    public static CourseCapacityProjection For(Guid courseId) => new (courseId, CourseChangeCapacityCondition.For(courseId));
    public CourseChangeCapacityCondition Condition => condition;
    public (CourseCapacity State, CourseChangeCapacityCondition Condition) Apply(DomainEvent[] events)
    {
        var state = new CourseCapacity(courseId, 0);
        foreach (var @event in events)
            switch (@event.Data)
            {
                case CourseDefined defined:
                    state = state with { Capacity = defined.Capacity };
                    break;
                case CourseCapacityChanged capacityChanged:
                    state = state with { Capacity = capacityChanged.Capacity };
                    break;
                default:
                    break;
            }
        return (state, condition);
    }
}