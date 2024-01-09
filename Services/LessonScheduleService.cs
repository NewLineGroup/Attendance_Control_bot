using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class LessonScheduleService : BaseService<Lesson>
{
    public LessonScheduleService(LessonRepository repository) : base(repository)
    {
    }
    
    public async Task<Lesson?> GetLesson(long lessonTime,string classNumber)
    {
      return  GetAllAsync().Result
            .FirstOrDefault(lesson => lesson.LessonTime == lessonTime && lesson.ClassNumber == classNumber);
    }
}