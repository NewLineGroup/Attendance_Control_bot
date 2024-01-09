using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceControlBot.Domain.Entity;

[Table("parents")]
public class Parent : BaseEntity
{ 
    [Column("chils_id")] public long ChildId { get; set; }
    [Column("auth_id")] public long AuthId { get; set; }
    [Column("is_stopped")] public bool IsStopped { get; set; }
    [Column("telegram_chat_id")] public long TelegramChatId { get; set; }
}