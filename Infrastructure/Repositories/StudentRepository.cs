using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Infrastructure.Repositories;

public class StudentRepository : RepositoryBase<Student>
{
    public StudentRepository(DataContext context) : base(context)
    {
    }
    public async Task<List<Student>> AddAllAsync(List<Student> students)
    {
         await _context.Set<Student>().AddRangeAsync(students);
         await _context.SaveChangesAsync();
         return students;
    }
    
    public async Task DeleteStudentsByClassNumber(string classNumber)
    {
        var studentsToDelete = _context.Students.Where(s => s.ClassNumber == classNumber);
        _context.Students.RemoveRange(studentsToDelete);
        _context.SaveChanges();
    }

   
    public async Task<List<string>> GetClassesByClassNumber()
    {
        List<Student> students = GetAll().ToList();
        
        Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();

        foreach (var student in students)
        {
            if (!groups.ContainsKey(student.ClassNumber))
            {
                groups[student.ClassNumber] = new List<string>();
            }
            groups[student.ClassNumber].Add(student.FirstName);
        }

        List<string> groupNames = groups.Keys.ToList();

        return groupNames;
    }
}