using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceControlBot.Domain.Entity;

public class BaseEntity
{
    [Column("Id")]
    public long Id { get; set; }
}