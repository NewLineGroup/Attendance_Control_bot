using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Domain.Enums;
using OfficeOpenXml;
namespace AttendanceControlBot.Services;

public static class ExcelService
{
   public static Task<List<Student>> ReadStudentsFromExcel(Stream stream)
    {
        List<Student> students = new List<Student>();

        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                Student student = new Student
                {
                    FirstName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                    LastName = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                    ParentAuthId = Convert.ToInt64(worksheet.Cells[row, 4].Value),
                    ClassNumber = worksheet.Cells[row, 5].Value?.ToString() ?? ""
                };

                students.Add(student);
            }
        }

        return Task.FromResult(students);
    }

   //Dars jadvalini exceldan yechib olsih uchun
   public static Task<List<Lesson>> ReadLessonSchedule(Stream stream)
    {
        List<Lesson> lessons = new List<Lesson>();

        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                Lesson lesson = new Lesson()
                {
                    LessonName = worksheet.Cells[row, 2].Value.ToString() ?? string.Empty,
                    LessonTime = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                    LessonDay =Enum.Parse<WeekDays>(worksheet.Cells[row, 4].Value.ToString()),
                    ClassNumber = worksheet.Cells[row, 5].Value.ToString() ?? string.Empty
                };

                lessons.Add(lesson);
            }
        }

        return Task.FromResult(lessons);
    }

   //Barcha studentlar ro'yxatini excel jadval sifatida qaytarish uchun
   public static async Task<Stream> ExportStudentsToExcelAndSendAsync(List<Student> students)
   {
       var excelPackage = new ExcelPackage();
       var worksheet = excelPackage.Workbook.Worksheets.Add("O'quvchilar ro'yxati");

       worksheet.Cells.LoadFromCollection(students, true);
       
       using (var stream = new MemoryStream())
       {
           excelPackage.SaveAs(stream);
           stream.Position = 0;
           
           return stream;
       }
   }
}
