using AttendanceControlBot.Domain.Dtos.StudentDtos;
using AttendanceControlBot.Domain.Dtos.WorkerDtos;
using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Domain.Enums;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class AdminService
{
    private WorkerRepository _workerRepository;
    public StudentRepository _studentRepository;
   

    public AdminService(WorkerRepository workerRepository,StudentRepository studentRepository)
    {
        _workerRepository = workerRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Worker> AddTeacher(TeacherCreationDto dto)
    {
        Worker teacher=new Worker()
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Password = dto.Password,
            TelegramChatId = 0,
            Signed = false,
            LastLoginDate = default,
            Subject = dto.Subject
        };
        await _workerRepository.AddAsync(teacher);
        return teacher;
    }
    
    public async Task<Worker> AddAdmin(WorkerCreationDto dto)
    {
        Worker worker=new Worker
        {
            PhoneNumber = dto.PhoneNumber,
            Password = dto.Password,
            TelegramChatId = 0,
            Signed = false,
            LastLoginDate = default,
            Role = Role.Admin
        };
        await _workerRepository.AddAsync(worker);
        return worker;
    }

    public async Task<long> AddSingleStudent(StudentCreationDto dto)
    {
        Student student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            ParentAuthId = await GenerateParentAuthId(),
            ClassNumber = dto.ClassNumber
        };
        await _studentRepository.AddAsync(student);
        return student.ParentAuthId;
    }


    public async Task AddManyStudents()
    {
       //no initialized 
    } 
    
    public async Task<List<Student>> GetStudentsParentAuthId(string classNumber)
    {
       return _studentRepository.GetAll().Where(student => student.ClassNumber == classNumber).ToList();
    }
    
    public async Task<long> GenerateParentAuthId()
    {
        long id =  _studentRepository.GetAll().Max(student=>student.ParentAuthId);
        return ++id;
    }

    public async Task<Student> DeleteStudent(Student student)
    {
       var deletedStudent= _studentRepository.GetAll().FirstOrDefault(s =>
            s.ClassNumber == student.ClassNumber && s.FirstName == student.FirstName && s.LastName == student.LastName);
     return await _studentRepository.RemoveAsync(deletedStudent);
    }
     public async Task<Worker> DeleteTeacher(Worker worker)
     {
         var teacher = _workerRepository.GetAll().FirstOrDefault(t => t.FullName==worker.FullName);
        return await _workerRepository.RemoveAsync(teacher);
    }
     
     public async Task DeleteClass(string classNumber)
     {
          await _studentRepository.DeleteStudentsByClassNumber(classNumber);
     }

     public async Task<bool> CheckClassNumber(string classNumber)
     {
         var res=_studentRepository.GetAll().Where(student => student.ClassNumber == classNumber).ToList();
         if (res is not null)
             return true;
         else
             return false;
     }
     
     
     public async Task<Stream> GetAllStudents()
     {
         return await ExcelService.ExportStudentsToExcelAndSendAsync(_studentRepository.GetAll().ToList());
     }
}