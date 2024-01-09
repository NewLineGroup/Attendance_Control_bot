using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Infrastructure.Repositories;

public class ParentRepository : RepositoryBase<Parent>
{
    public ParentRepository(DataContext context) : base(context)
    {
    }
}