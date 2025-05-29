namespace DynamicConsistencyBoundary.Tests.CourseSubscriptionExample.Application;

public record CourseDefined(Guid CourseId, int Capacity);
public record CourseCapacityChanged(Guid CourseId, int Capacity);