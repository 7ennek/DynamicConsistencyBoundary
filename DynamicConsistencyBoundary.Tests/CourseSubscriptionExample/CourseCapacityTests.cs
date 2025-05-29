using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;
using DynamicConsistencyBoundary.Tests.Framework;
using DynamicConsistencyBoundary.Tests.TestHelpers;
using Shouldly;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample;

public class CourseCapacityTests
{
    private readonly InMemoryEventStore _eventStore = new();

    [Fact]
    public void updated_capacity_should_be_projected()
    {
        var someCourseId = Some.Guid;
        _eventStore.Append(
            new CourseDefined(someCourseId, Some.Integer),
            [DomainIdentifier.For(someCourseId, "Course")],
            CourseExistsCondition.For(someCourseId)
        );

        var updateCapacity = Some.Integer;
        _eventStore.Append(
            new CourseCapacityChanged(someCourseId, updateCapacity),
            [DomainIdentifier.For(someCourseId, "Course")],
            0,
            CourseChangeCapacityCondition.For(someCourseId)
        );
        
        var (projectResult, _) = _eventStore.Project(CourseCapacityProjection.For(someCourseId));
        projectResult.ShouldNotBeNull();
        projectResult.Capacity.ShouldBe(updateCapacity);
    }
}