using AttendanceControlBot.Domain.Dtos.AuthDto;
using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Domain.SessionModels;

public class Session
{
    public long Id { get; set; }
    public Worker Worker { get; set; } = new Worker();
    public string Action { get; set; }
    public string Controller { get; set; }
    public long ChatId { get; set; }
    public Student Student { get; set; }
    public Worker Teacher { get; set; } = new Worker();
    public LoginSessionModel LoginData { get; set; }
    public byte LessonTime { get; set; }
    public string? ClassNumber { get; set; }
    public string? InlineResultQueryId { get; set; }
}