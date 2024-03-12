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

  
    public async Task<List<Student>> GetParentAllChildren(long parentTelegramChatId)
    {
        // Get the parent IDs based on the Telegram chat ID
        var parent = _parentRepository
            .GetAll()
            .Where(parent => parent.TelegramChatId == parentTelegramChatId).Select(parent=>parent.ChildId).ToList();

        // Get all students whose parent IDs match the retrieved parent IDs
        var children = _studentRepository.GetAll().Where(student => parent.Contains(student.Id)).ToList();

        return children;
    }

    public async Task DeleteParentByTelegramChatId(long telegramChatId)
    {
        foreach (Parent parent in _parentRepository.GetAll().ToList())
        {
            if (parent.TelegramChatId==telegramChatId)
                await _parentRepository.RemoveAsync(parent);
        }
    }
}