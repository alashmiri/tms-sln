using Microsoft.AspNetCore.Authentication;
using TmsApi;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
var builder = WebApplication.CreateBuilder(args);
// Register TmsDbContext scoped for incoming HTTP requests
builder.Services.AddControllers();
builder.Services.AddDbContext<TmsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"))
.LogTo(Console.WriteLine, LogLevel.Information) // Log SQL to output window
.EnableSensitiveDataLogging()); // Show parameters in querylogs (dev only)

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
app.MapControllers();

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

// In-memory seed data (for quick endpoint testing)
var students = new List<TmsApi.Entities.Student>
{
    new() { Id = 1, RegistrationNumber = "S-001", Name = "Alice Smith", GPA = 3.8m, IsActive = true },
    new() { Id = 2, RegistrationNumber = "S-002", Name = "Bilal Jones", GPA = 3.2m, IsActive = true },
    new() { Id = 3, RegistrationNumber = "S-003", Name = "Chala Brown", GPA = 3.5m, IsActive = true },
};

var courses = new List<TmsApi.Entities.Course>
{
    new() { Id = 1, Code = "CS-101", Title = "Intro to Programming", Capacity = 30 },
    new() { Id = 2, Code = "CS-201", Title = "Data Structures", Capacity = 25 },
    new() { Id = 3, Code = "CS-301", Title = "Algorithms", Capacity = 20 },
};

// Student endpoints
app.MapGet("/students/all", () => Results.Ok(students));

app.MapGet("/students/{id:int}", (int id) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});

// Course endpoints
app.MapGet("/courses/all", () => Results.Ok(courses));

app.MapGet("/courses/{id:int}", (int id) =>
{
    var course = courses.FirstOrDefault(c => c.Id == id);
    return course is not null ? Results.Ok(course) : Results.NotFound();
});


// Seed test data at startup
using (var scope = app.Services.CreateScope())
{
var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
context.Database.Migrate(); // Applies any pending migrations; keeps migration history intact
if (!context.Students.Any())
{
var seedStudents = new List<TmsApi.Entities.Student>
{
    new() { RegistrationNumber = "TMS-2026-0001", Name = "AliceSmith", GPA = 3.8m, IsActive = true },
    new() { RegistrationNumber = "TMS-2026-0002", Name = "Bob Jones", GPA = 2.9m, IsActive = true },
    new() { RegistrationNumber = "TMS-2026-0003", Name = "Charlie Brown", GPA = 3.4m, IsActive = false },
    new() { RegistrationNumber = "TMS-2026-0004", Name = "DianaPrince", GPA = 3.9m, IsActive = true },
    new() { RegistrationNumber = "TMS-2026-0005", Name = "EvanWright", GPA = 2.5m, IsActive = true }
};
context.Students.AddRange(seedStudents);
var seedCourses = new List<TmsApi.Entities.Course>
{
    new() { Code = "CS-101", Title = "Introduction to ComputerScience", Capacity = 30 },
    new() { Code = "CS-201", Title = "Data Structures and Algorithms", Capacity = 25 },
    new() { Code = "MAT-101", Title = "Calculus I", Capacity = 40 }
};
context.Courses.AddRange(seedCourses);
context.SaveChanges();
var seedEnrollments = new List<TmsApi.Entities.Enrollment>
{
    new() { StudentId = seedStudents[0].Id, CourseId = seedCourses[0].Id, Grade = 4.0m },
    new() { StudentId = seedStudents[0].Id, CourseId = seedCourses[1].Id, Grade = 3.6m },
    new() { StudentId = seedStudents[1].Id, CourseId = seedCourses[0].Id, Grade = 2.8m },
    new() { StudentId = seedStudents[3].Id, CourseId = seedCourses[1].Id, Grade = 3.9m }
};
context.Enrollments.AddRange(seedEnrollments);
context.SaveChanges();
}
}

app.Run();

public record EnrollRequest(string StudentId, string CourseCode);




