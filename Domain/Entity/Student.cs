using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceControlBot.Domain.Entity;

[Table("students")]
public class Student:BaseEntity
{
    [Column("first_name")] public string FirstName { get; set; }
    [Column("last_name")] public string LastName { get; set; }
    [Column("parent_auth_id")] public long ParentAuthId { get; set; }
    [Column("class_number")] public string ClassNumber { get; set; }
}