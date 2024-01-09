using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class WorkerService : BaseService<Worker>
{
    public WorkerService(RepositoryBase<Worker> repository) : base(repository)
    {
    }
}