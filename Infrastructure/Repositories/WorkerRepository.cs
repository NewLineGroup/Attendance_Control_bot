using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Infrastructure.Repositories;

public class WorkerRepository : RepositoryBase<Worker>
{
    public WorkerRepository(DataContext context) : base(context)
    {
    }
    public async Task<List<Worker>> AddAllAsync(List<Worker> workers)
    {
        await _context.Set<Worker>().AddRangeAsync(workers);
        await _context.SaveChangesAsync();
        return workers;
    }
}