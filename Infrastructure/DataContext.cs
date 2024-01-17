using AttendanceControlBot.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceControlBot.Infrastructure;

public class DataContext : DbContext
{
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Parent> Parents { get; set; }
   
    public DataContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies();
        optionsBuilder.UseNpgsql("Host=213.230.65.55; Port=5444; Database=postgres; username=postgres; password=159357Dax;");
        base.OnConfiguring(optionsBuilder);
    }
}