using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Infrastructure.Repositories;

public class WorkerRepository : RepositoryBase<Worker>
{
    public WorkerRepository(DataContext context) : base(context)
    {
    }
}