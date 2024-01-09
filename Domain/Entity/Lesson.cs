using System.ComponentModel.DataAnnotations.Schema;
using AttendanceControlBot.Domain.Enums;

namespace AttendanceControlBot.Domain.Entity;
[Table("lessons")]
public class Lesson : BaseEntity
{
    [Column("lesson_name")] public string LessonName { get; set; }
    [Column("lesson_time")] public int LessonTime { get; set; }
    [Column("lesson_day")] public WeekDays LessonDay { get; set; }
    [Column("class_number")] public string ClassNumber { get; set; }
}