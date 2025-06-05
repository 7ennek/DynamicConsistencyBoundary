using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;
using DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application.Projections;
using DynamicConsistencyBoundary.Tests.Framework;
using DynamicConsistencyBoundary.Tests.TestHelpers;
using Shouldly;

namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Tests.Projections;

public class CourseDefiningTests
{
    private readonly InMemoryEventStore _eventStore = new();
    
    [Fact]
    public void be_able_to_define_a_new_course_when_not_existing()
    {
        Should.NotThrow(() =>  
            AppendCourseDefinedToStore(Some.Guid)
        );
    }

    [Fact]
    public void defining_a_new_course_when_already_exists_should_not_be_possible()
    {
        var someCourseId = Some.Guid;
        AppendCourseDefinedToStore(someCourseId);
        Should.Throw<InvalidOperationException>(() =>  
            AppendCourseDefinedToStore(someCourseId)
        );   
    }

    [Fact]
    public void projection_on_existing_course_should_give_back_correct_state()
    {
        var someCourseId = Some.Guid;
        AppendCourseDefinedToStore(someCourseId);
        var (projectResult, _) = _eventStore.Project(CourseExistsProjection.For(someCourseId));
        projectResult.ShouldNotBeNull();
        projectResult.Exists.ShouldBeTrue();
    }

    [Fact]
    public void projection_on_nonexisting_course_should_give_back_correct_state()
    {
        var (projectResult, _) = _eventStore.Project(CourseExistsProjection.For(Some.Guid));
        projectResult.ShouldNotBeNull();
        projectResult.Exists.ShouldBeFalse();
    }
    
    private void AppendCourseDefinedToStore(Guid someCourseId)
    {
        var newCourse = new CourseDefined(someCourseId, Some.Integer);
        _eventStore.Append(
            newCourse,
            [DomainIdentifier.For(newCourse.CourseId, "Course")],
            CourseExistsCondition.For(someCourseId)
        );
    }
}