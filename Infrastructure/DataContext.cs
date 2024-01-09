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
    //"host=213.230.65.55;port=5444;username=postgres;password=159357Dax;database=tcb_app"
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies();
        optionsBuilder.UseNpgsql("host=localhost;port=5432;username=postgres;password=3214;database=control_bot");
        base.OnConfiguring(optionsBuilder);
    }
}