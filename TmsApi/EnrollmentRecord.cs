namespace TmsApi;
//--- The data shape--
public record EnrollmentRecord(
    string Id,
    string StudentId,
    string CourseCode,
    DateTime CreatedAt);