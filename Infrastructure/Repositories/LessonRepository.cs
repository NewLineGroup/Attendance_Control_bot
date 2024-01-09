using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Infrastructure.Repositories;

public class LessonRepository : RepositoryBase<Lesson>
{
    public LessonRepository(DataContext context) : base(context)
    {
    }
    public async Task<List<Lesson>> AddAllAsync(List<Lesson> lessons)
    {
        await _context.Set<Lesson>().AddRangeAsync(lessons);
        await _context.SaveChangesAsync();
        return lessons;
    }
}