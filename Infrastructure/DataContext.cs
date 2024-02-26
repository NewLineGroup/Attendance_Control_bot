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
        optionsBuilder.UseNpgsql("Host=viaduct.proxy.rlwy.net; Port=50688; Database=railway; username=postgres; password=EgcA3526cBffD442geg*c4eC24a1bF5A;");
        base.OnConfiguring(optionsBuilder);
    }
    //roundhouse.proxy.rlwy.net:43832
}