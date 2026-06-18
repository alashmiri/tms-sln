using Microsoft.AspNetCore.Authentication;

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
app.Run();*/

// exercise only about the authentication

/*var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("TmsScheme")
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
.RequireAuthorization();
app.Run();
