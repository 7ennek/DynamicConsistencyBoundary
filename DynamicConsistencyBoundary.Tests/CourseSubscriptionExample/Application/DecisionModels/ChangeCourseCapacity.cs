using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application.Projections;
using DynamicConsistencyBoundary.Tests.Framework;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application.DecisionModels;

public class ChangeCourseCapacity(Guid courseId, int updateToCapacity) 
    : IDecision<(bool Exists, int Capacity), ConditionWrapper>
{
    private readonly CourseExistsProjection _courseExistsProjection = CourseExistsProjection.For(courseId);
    private readonly CourseCapacityProjection _courseCapacityProjection = CourseCapacityProjection.For(courseId);
    
    public ConditionWrapper Condition => 
        ConditionWrapper.For([
            _courseExistsProjection.Condition,
            _courseCapacityProjection.Condition,
        ]);
    
    public ((bool Exists, int Capacity) State, ConditionWrapper Condition) Apply(DomainEvent[] domainEvents)
    {
        return (
            (
                _courseExistsProjection.Apply(domainEvents).State.Exists,
                _courseCapacityProjection.Apply(domainEvents).State.Capacity
            ), 
            Condition
        );
    }

    public (bool IsSatisfied, object[] ToApply, DomainIdentifier DomainIdentifier) ApplyWhenSatisfiedWith((bool Exists, int Capacity) state)
    {
        return (
            state.Exists && state.Capacity != updateToCapacity,
            [new CourseCapacityChanged(courseId, updateToCapacity)],
            DomainIdentifier.For(courseId, "Course")
        );
    }
    
    public static ChangeCourseCapacity For(Guid courseId, int updateToCapacity) => new (courseId,  updateToCapacity);
}