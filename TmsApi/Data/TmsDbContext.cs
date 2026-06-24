using Microsoft.EntityFrameworkCore;
using TmsApi.Entities;
namespace TmsApi.Data;
public class TmsDbContext(DbContextOptions<TmsDbContext> options) : DbContext(options)
{

public DbSet<Entities.Student> Students => Set<Entities.Student>();
public DbSet<Entities.Course> Courses => Set<Entities.Course>();
public DbSet<Entities.Enrollment> Enrollments => Set<Entities.Enrollment>();
public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}