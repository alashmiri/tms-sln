using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TmsApi;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddSingleton<EnrollmentWorker>();

// Authentication — TmsScheme always fails => 401 for protected endpoints
builder.Services.AddAuthentication("TmsScheme")
    .AddScheme<AuthenticationSchemeOptions, TmsAssessmentAuthHandler>("TmsScheme", null);
builder.Services.AddAuthorization();

// Validate PaymentOptions on startup (fail fast when GatewayUrl is missing)
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

app.Run();

public record EnrollRequest(string StudentId, string CourseCode);




//First, make the failure visible
//Step A Buggy registration (temporary): Implement EnrollmentWorker so its constructor
//takes IEnrollmentService directly (not IServiceScopeFactory yet). Register:
//builder.Services.AddSingleton<EnrollmentWorker>();
//builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
// Change this line:
 //builder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();
 //builder.Services.AddSingleton<EnrollmentService>();
 //Addhost validation so the container catches illegal lifetime wiring early:
/*builder.Host.UseDefaultServiceProvider(options =>
{
options.ValidateScopes = true;
options.ValidateOnBuild = true;
});
var app = builder.Build();
app.MapGet("/", () => "TMS System Online");*/

// Bind, Validate Data Annotations, and Enforce Fail-Fast on Startup
/*builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart(); */



/*// Starter pipeline (do not assume this order is correct) 

//var builder = WebApplication.CreateBuilder(args);

app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
});
app.Run();*/

// exercise only about the authentication

//var builder = WebApplication.CreateBuilder(args);
/*builder.Services.AddAuthentication("TmsScheme")
    .AddScheme<AuthenticationSchemeOptions, TmsAssessmentAuthHandler>("TmsScheme", null);
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
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
app.Run();*/

// exercise about the middlewre plus correlation id

//var builder = WebApplication.CreateBuilder(args);
// auth services
/*builder.Services.AddAuthentication("TmsScheme")
    .AddScheme<AuthenticationSchemeOptions, TmsAssessmentAuthHandler>("TmsScheme", null);
builder.Services.AddAuthorization();
var app = builder.Build();
// middleware order
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// endpoint 
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
})
.RequireAuthorization();*/
//app.Run();


