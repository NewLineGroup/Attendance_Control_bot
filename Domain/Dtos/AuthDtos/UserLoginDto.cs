namespace AttendanceControlBot.Domain.Dtos.AuthDtos;

public class UserLoginDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public long TelegramChatId { get; set; }
}