using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;
using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application.DecisionModels;
using DynamicConsistencyBoundary.Tests.Framework;
using DynamicConsistencyBoundary.Tests.TestHelpers;
using Shouldly;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Tests.DecisionModels;

public class ChangeCourseCapacityTests
{
    private readonly InMemoryEventStore _eventStore = new();
    
    [Fact]
    public void be_able_to_create_decision_model_based_on_multiple_projections()
    {
        var someCourseId = Some.Guid;
        _eventStore.Append(
            new CourseDefined(someCourseId, Some.Integer),
            [DomainIdentifier.For(someCourseId, "Course")]
        );

        var updateCapacity = Some.Integer;
        _eventStore.Append(
            new CourseCapacityChanged(someCourseId, updateCapacity),
            [DomainIdentifier.For(someCourseId, "Course")]
        );
        
        var (decision, _) = _eventStore.Project(ChangeCourseCapacity.For(someCourseId, Some.Integer));
        decision.Exists.ShouldBeTrue();
        decision.Capacity.ShouldBe(updateCapacity);
    }

    [Fact]
    public void applies_events_when_satisfied()
    {
        var someCourseId = Some.Guid;
        _eventStore.Append(
            new CourseDefined(someCourseId, Some.Integer),
            [DomainIdentifier.For(someCourseId, "Course")]
        );

        var updateCapacity = Some.Integer;
        _eventStore.Append(
            new CourseCapacityChanged(someCourseId, updateCapacity),
            [DomainIdentifier.For(someCourseId, "Course")]
        );
        
        _eventStore.ApplyWhenSatisfied(ChangeCourseCapacity.For(someCourseId, Some.Integer));
    }
}