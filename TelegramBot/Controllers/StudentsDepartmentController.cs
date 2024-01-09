using AttendanceControlBot.Domain.Dtos.StudentDtos;
using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Infrastructure.Repositories;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class StudentsDepartmentController : ControllerBase
{
    private string? _classNumber = null;
    private AdminService adminService;
    public StudentsDepartmentController(ControllerManager.ControllerManager controllerManager, AdminService adminService) : base(controllerManager)
    {
        this.adminService = adminService;
    }

    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            case nameof(Index):
                await this.Index(context);
                break;
            case nameof(AddStudentStart):
                await this.AddStudentStart(context);
                break;
            case nameof(AddStudentFirstName):
                await this.AddStudentFirstName(context);
                break;
            case nameof(AddStudentLastName):
                await this.AddStudentLastName(context);
                break;
            case nameof(AddStudentClassNumber):
                await this.AddStudentClassNumber(context);
                break;
            case nameof(DeleteStudentStart):
                await this.DeleteStudentStart(context);
                break; 
            case nameof(DeleteStudentGetFullName):
                await this.DeleteStudentGetFullName(context);
                break; 
            case nameof(DeleteStudentGetClassNumber):
                await this.DeleteStudentGetClassNumber(context);
                break;
            case nameof(AddStudentFromExcel):
                await this.AddStudentFromExcel(context);
                break;
            case nameof(DeleteClassStart):
                await this.DeleteClassStart(context);
                break;
            case nameof(DeleteClassGetStudentClassNumber):
                await this.DeleteClassGetStudentClassNumber(context);
                break;
            case nameof(DeleteClassConfirmation):
                await this.DeleteClassConfirmation(context);
                break;
            case nameof(GetAllStudents):
                await this.GetAllStudents(context);
                break;
        }
    }
    

    protected override async Task HandleUpdate(ControllerContext context)
    {
        if (context.Message?.Type is MessageType.Text)
        {
            string text = context.Message?.Text;
            switch (text)
            {
                case "O'quvchi qo'shish":
                    context.Session.Action = nameof(AddStudentStart);
                    break;
                case "Excel orqali qo'shish":
                    context.Session.Action = nameof(AddStudentFromExcel);
                    break;
                case "O'quvchi o'chirish":
                    context.Session.Action = nameof(DeleteStudentStart);
                    break;
                case "Sinfni o'chirish":
                    context.Session.Action = nameof(DeleteClassStart);
                    break;
                case "Xa✅":
                    context.Session.Action = nameof(DeleteClass);
                    break; 
                case "Yo'q❌":
                    context.Session.Action = nameof(Index);
                    break;
                case "Ortga":
                    context.Session.Controller = nameof(AdminDashboardController);
                    context.Session.Action = nameof(AdminDashboardController.Index);
                    break;
            }
        }
        if (context.Message.Document != null && context.Session.Action==nameof(AddStudentFromExcel))
        {
            var fileId = context.Message.Document.FileId;
            //var fileName = context.Message.Document.FileName;

            using (var fileStream = new MemoryStream())
            {
                var file = await _botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
                fileStream.Position = 0;

                var students =await ExcelService.ReadStudentsFromExcel(fileStream);
                if (students is not null)
                {
                    var res = await adminService._studentRepository.AddAllAsync(students);
                    await context.SendBoldTextMessage("Muvofaqqiyatli qo'shildi✅", context.StudentsDepartmentReplyKeyboardMarkup());
                    context.Session.Action = nameof(StudentsDepartmentController.Index);
                }
                else
                    await context.SendBoldTextMessage(@"Siz yuborgan excel file mos emas! 
 Iltimos file bosh emasligi va ma'lumotlar berilgan na'muna kabi ekanligin tekshiring❌", 
                        context.StudentsDepartmentReplyKeyboardMarkup());
            }
        }
    }

    public async Task Index(ControllerContext context)
    {
        await context.SendBoldTextMessage("Tanlang: ",context.StudentsDepartmentReplyKeyboardMarkup());
    }
    

    private async Task AddStudentStart(ControllerContext context)
    {
        context.Session.Action = nameof(AddStudentFirstName);
        await context.SendBoldTextMessage("O'quvchi ismini kiriting: ");
    }

    private async Task AddStudentFirstName(ControllerContext context)
    {
        context.Session.Student = new Student
        {
            FirstName = context.Update.Message.Text,
        };
        context.Session.Action = nameof(AddStudentLastName);
        await context.SendBoldTextMessage("O'quvchi familiyasini kiriting: ");

    } 
    private async Task AddStudentLastName(ControllerContext context)
    {
        context.Session.Student.LastName = context.Update.Message.Text;
        context.Session.Action = nameof(AddStudentClassNumber);
        await context.SendBoldTextMessage("O'quvchining sinf raqamini kiriting: ");
    }
    
    private async Task AddStudentClassNumber(ControllerContext context)
    {
        context.Session.Student.ClassNumber = context.Update.Message.Text;
        context.Session.Action = nameof(Index);
      
      var res=  await  adminService.AddSingleStudent(new StudentCreationDto
        {
            FirstName = context.Session.Student.FirstName,
            LastName = context.Session.Student.LastName,
            ClassNumber = context.Session.Student.ClassNumber
        });
        
      await context.SendBoldTextMessage($"Muvofaqqiyatli qo'shildi✅.\n O'quvching ota-onasining tasdiqlash idsi: {res}",context.StudentsDepartmentReplyKeyboardMarkup());
      context.Session.Student = null;
    }

    private async Task AddStudentFromExcel(ControllerContext context)
    {
        await context.SendTextMessage("Excel fileni yuboring");
    }
    private async Task DeleteStudentStart(ControllerContext context)
    {
        context.Session.Action = nameof(DeleteStudentGetFullName);
        await context.SendBoldTextMessage("O'quvchi ismini va familiyasini kiriting. \n Na'muna: G'olibjon Abdurasulov");
    }

    private async Task DeleteStudentGetFullName(ControllerContext context)
    {
        string[] fullName = context.Update.Message.Text.Split(' ', 2);
        context.Session.Student = new Student()
        {
            FirstName = fullName[0],
            LastName = fullName[1]
        };
        context.Session.Action = nameof(DeleteStudentGetClassNumber);
        await context.SendBoldTextMessage("O'quvchi sinf raqamini kiriting: ");  
    }

    private async Task DeleteStudentGetClassNumber(ControllerContext context)
    {
        context.Session.Student.ClassNumber = context.Update.Message.Text;
        await adminService.DeleteStudent(context.Session.Student);
        context.Session.Student = null;
        await context.SendBoldTextMessage("Muvofaqqiyatli o'chirildi✅", context.MakeAdminDashboardReplyKeyboardMarkup());
    }

    private async Task DeleteClassStart(ControllerContext context)
    {
        await context.SendBoldTextMessage("O'chirmoqchi bo'lgan sinfingizni raqamini kiriting");
        context.Session.Action = nameof(DeleteClassGetStudentClassNumber);
    }
    
    private async Task DeleteClassGetStudentClassNumber(ControllerContext context)
    {
        _classNumber = context.Update.Message.Text;
        await context.SendBoldTextMessage("Rostdan sinfni o'chirmoqchimisz: ",replyMarkup: context.MakeDeleteClassConfirmation());
        context.Session.Action = nameof(DeleteClassConfirmation);
    }
    
    private async Task DeleteClassConfirmation(ControllerContext context)
    {
        _classNumber = context.Update.Message.Text;
        var isValid =await CheckClassNumber(_classNumber);
        if (isValid)
        {
            await context.SendBoldTextMessage("Rostdan sinfni o'chirmoqchimisz: ",
                replyMarkup: context.MakeDeleteClassConfirmation());
        }
        else
        {
            await context.SendBoldTextMessage("Siz kiritgan sinf raqami yaroqli emas yoki bunday sinf raqamiga ega o'quvchilar mavjud emas: ",context.StudentsDepartmentReplyKeyboardMarkup());
            context.Session.Action = nameof(Index);
        }
    }
    
    private async Task DeleteClass(ControllerContext context)
    {
        await adminService.DeleteClass(_classNumber);
        await context.SendBoldTextMessage("Muvoffaqqiyatli o'chirildi",context.StudentsDepartmentReplyKeyboardMarkup());
    }
    
    private async Task<bool> CheckClassNumber(string classNumber)
    {
        var res=await adminService.CheckClassNumber(classNumber);
        if (classNumber is not null && classNumber.Length <=2&&res)
        {
            return true;
        }
        return false;
    }

    private async Task GetAllStudents(ControllerContext context)
    {
        var stream=await adminService.GetAllStudents();
        await _botClient.SendDocumentAsync(context.Message.Chat.Id, document: new InputFileStream(stream, "students.xlsx"));
        context.Session.Action = nameof(Index);
    }
}