using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication;

/*// Starter pipeline (do not assume this order is correct) 
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
});
//app.UseAuthentication(); 
//app.UseAuthorization();
app.Run();*/


/*var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Fake")
    .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Fake", null);

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



var builder = WebApplication.CreateBuilder(args);

// auth services
builder.Services.AddAuthentication("TmsScheme")
    .AddScheme<AuthenticationSchemeOptions, TmsAssessmentAuthHandler>("TmsScheme", null);

builder.Services.AddAuthorization();

var app = builder.Build();

// middleware order
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ONLY ONE endpoint (no duplicates)
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

app.Run();
