using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Infrastructure.Repositories;

namespace AttendanceControlBot.Services;

public class ParentService : BaseService<Parent>
{
    private ParentRepository _parentRepository;
    private StudentRepository _studentRepository;

    public ParentService(ParentRepository parentRepository, StudentRepository studentRepository) : base(
        parentRepository)
    {
        _parentRepository = parentRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Student?> CheckParentAuthId(long parentAuthId)
    {
        var res = _studentRepository.GetAll().FirstOrDefault(student => student.ParentAuthId == parentAuthId);
        return res;
    }

    public async Task<Parent?> GetParentByStudentId(long studentId)
    {
        var res = _parentRepository.GetAll().FirstOrDefault(p => p.ChildId == studentId&&!p.IsStopped);
        return res;
    }

    public async Task<List<Student>> GetParentAllChildes(long parentTelegramChatId)
    {
      var parentIds = _parentRepository
                .GetAll()
                .Where(parent => parent.TelegramChatId == parentTelegramChatId)
                .Select(parent => parent.Id)
                .ToList();

            var childIds = _studentRepository
                .GetAll()
                .Where(student => parentIds.Contains(student.ParentAuthId))
                .ToList();
        return childIds;
    }

}