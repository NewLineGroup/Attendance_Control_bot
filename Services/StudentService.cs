using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class StudentService : BaseService<Student>
{
    private StudentRepository _studentRepository;
    public StudentService(StudentRepository repository) : base(repository)
    {
        _studentRepository = repository;
    }

    public async Task<List<string>> GetAllClassNumbers()
    {
       return await _studentRepository.GetClassesByClassNumber();
    }
    public async Task<List<Student>> GetStudentsByClassNumber(string classNumber)
    {
       return  _studentRepository.GetAll().Where(s=>s.ClassNumber==classNumber).ToList();
    }
    public async Task<string> GetClassNumber(string classNumber)
    {
       var numbers= await GetAllClassNumbers();
       return numbers.FirstOrDefault(number => number == classNumber);
    }
}