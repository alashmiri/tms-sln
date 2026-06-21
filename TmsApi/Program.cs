using Microsoft.AspNetCore.Authentication;
using TmsApi;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();


// Authentication — TmsScheme always fails => 401 for protected endpoints
builder.Services.AddAuthentication("TmsScheme")
    .AddScheme<AuthenticationSchemeOptions, TmsAssessmentAuthHandler>("TmsScheme", null);
builder.Services.AddAuthorization();
builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Enforce DI lifetime validation
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var app = builder.Build();

// Middleware pipeline (outer → inner)
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "An error occurred" });
    });
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Protected endpoint — returns 401 for anonymous requests
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
})
.RequireAuthorization();

// Test endpoint for duplicate enrollment verification
app.MapPost("/api/enrollments", async (IEnrollmentService svc, EnrollRequest request) =>
{
    var first = await svc.EnrollAsync(request.StudentId, request.CourseCode);
    var second = await svc.EnrollAsync(request.StudentId, request.CourseCode);
    return Results.Ok(new { first, second, duplicateDetected = first.Id == second.Id });
});

// In-memory seed data
var students = new List<Student>
{
    new() { Id = "S-001", Name = "Ali", Age = 20, GPA = 3.8m },
    new() { Id = "S-002", Name = "Bilal", Age = 22, GPA = 3.2m },
    new() { Id = "S-003", Name = "Chala", Age = 19, GPA = 3.5m },
};

var courses = new List<Course>
{
    new() { Code = "CS-101", Title = "Intro to Programming", Capacity = 30, EnrolledCount = 25 },
    new() { Code = "CS-201", Title = "Data Structures", Capacity = 25, EnrolledCount = 20 },
    new() { Code = "CS-301", Title = "Algorithms", Capacity = 20, EnrolledCount = 18 },
};

// Student endpoints
app.MapGet("/students/all", () => Results.Ok(students));

app.MapGet("/students/{id}", (string id) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});

// Course endpoints
app.MapGet("/courses/all", () => Results.Ok(courses));

app.MapGet("/courses/{id}", (string id) =>
{
    var course = courses.FirstOrDefault(c => c.Code == id);
    return course is not null ? Results.Ok(course) : Results.NotFound();
});

app.Run();

public record EnrollRequest(string StudentId, string CourseCode);




