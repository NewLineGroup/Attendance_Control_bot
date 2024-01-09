using Telegram.Bot.Types;

namespace AttendanceControlBot.TelegramBot.Context;

public class BaseContext
{
    public Update Update { get; set; }
    public Message Message => Update?.Message;
}