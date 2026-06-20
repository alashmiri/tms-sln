namespace TmsApi;

public record EnrollmentRecord(
    string Id,
    string StudentId,
    string CourseCode,
    DateTime CreatedAt);