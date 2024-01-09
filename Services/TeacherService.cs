using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class TeacherService : BaseService<Worker>
{
    public TeacherService(WorkerRepository repository) : base(repository)
    {
    }

    public async Task<bool> SendMessageAbsenceFromLesson(Student student)
    {
        return true;
    }
}