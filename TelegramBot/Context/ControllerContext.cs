using AttendanceControlBot.Domain.SessionModels;

namespace AttendanceControlBot.TelegramBot.Context;

public class ControllerContext:BaseContext
{
    public Func<Task> TerminateSession { get; set; }
    public Session Session { get; set; }
}