using System.ComponentModel.DataAnnotations.Schema;
using AttendanceControlBot.Domain.Enums;

namespace AttendanceControlBot.Domain.Entity;
[Table("workers")]
public class Worker : BaseEntity
{
    [Column("full_name")] public string FullName { get; set; }
    [Column("phone_number")] public string PhoneNumber { get; set; }
    [Column("password")] public string Password { get; set; }
    [Column("telegram_chat_id")] public long TelegramChatId { get; set; }
    [Column("signed")] public bool Signed { get; set; }
    [Column("last_login_date")] public DateTime LastLoginDate { get; set; }
    [Column("role")] public Role Role{ get; set; }
    [Column("subject")] public string? Subject { get; set; }
}