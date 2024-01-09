namespace AttendanceControlBot.Domain.Dtos.AuthDto;

public class UserRegistrationDto
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public long ChatId { get; set; }
}