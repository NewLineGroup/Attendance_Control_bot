using AttendanceControlBot.Domain.Dtos.WorkerDtos;
using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class TeacherDepartmentController : ControllerBase
{
    private AdminService _adminService;

    public TeacherDepartmentController(ControllerManager.ControllerManager controllerManager, AdminService adminService) : base(controllerManager)
    {
        _adminService = adminService;
    }

    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            case nameof(Index):
                await this.Index(context);
                break;
            case nameof(AddTeacherStart):
                await this.AddTeacherStart(context);
                break;
            case nameof(AddTeacherFullName):
                await this.AddTeacherFullName(context);
                break;
            case nameof(AddTeacherPhoneNumber):
                await this.AddTeacherPhoneNumber(context);
                break;
            case nameof(AddTeacherPassword):
                await this.AddTeacherPassword(context);
                break;
            case nameof(AddTeacherSubject):
                await this.AddTeacherSubject(context);
                break; 
            case nameof(DeleteTeacherStart):
                await this.DeleteTeacherStart(context);
                break;
            case nameof(DeleteTeacherFinish):
                await this.DeleteTeacherFinish(context);
                break;
            case nameof(AddTeachers):
                await this.AddTeachers(context);
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
                case "O'qituvchi qo'shish":
                    context.Session.Action = nameof(AddTeacherStart);
                    break;
                case "O'qituvchini o'chirish":
                    context.Session.Action = nameof(DeleteTeacherStart);
                    break;
                case "O'qituvchilarni excel orqali qo'shish":
                    context.Session.Action = nameof(AddTeachers);
                    break;
                case "Ortga":
                    context.Session.Controller = nameof(AdminDashboardController);
                    context.Session.Action = nameof(AdminDashboardController.Index);
                    break;
            }
        }
        else if (context.Message.Document != null && context.Session.Action==nameof(AddTeachers))
        {
            var fileId = context.Message.Document.FileId;
            //var fileName = context.Message.Document.FileName;

            using (var fileStream = new MemoryStream())
            {
                var file = await _botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
                fileStream.Position = 0;

                var workers =await ExcelService.ReadWorkersFromExcel(fileStream);
                if (workers is not null)
                {
                    var res = await _adminService._workerRepository.AddAllAsync(workers);
                    await context.SendBoldTextMessage("Muvofaqqiyatli qo'shildi✅", context.TeachersDepartmentReplyKeyboardMarkup());
                    context.Session.Action = nameof(TeacherDepartmentController.Index);
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
        await context.SendBoldTextMessage("Tanlang: ",context.TeachersDepartmentReplyKeyboardMarkup());
    }

    public async Task AddTeacherStart(ControllerContext context)
    {
        context.Session.Action = nameof(AddTeacherFullName);
        await context.SendBoldTextMessage("O'qituvchi ismini va familiyasini kiriting: \n Na'muna: G'olibjon Abdurasulov");
    }

    public async Task AddTeacherFullName(ControllerContext context)
    {
        context.Session.Teacher = new Worker()
        {
            FullName = context.Update.Message.Text,
        };
        context.Session.Action = nameof(AddTeacherPhoneNumber);
        await context.SendBoldTextMessage("O'qituvchi telefon raqamini kiriting: \n Masalan:+998901092779");
    }

    public async Task AddTeacherPhoneNumber(ControllerContext context)
    {
        context.Session.Teacher.PhoneNumber = context.Update.Message.Text;
        context.Session.Action = nameof(AddTeacherPassword);
        await context.SendBoldTextMessage("O'qituvchi parolini kiriting: \n Eslatma: Parol 6 ta belgidan iborat bo'lishi kerak!");
    }
    
    public async Task AddTeacherPassword(ControllerContext context)
    {
        context.Session.Teacher.Password = context.Update.Message.Text;
        context.Session.Action = nameof(AddTeacherSubject);
        await context.SendBoldTextMessage("O'qituvchi dars beradigan fanni kiriting: \n Na'muna: Ona tili");
    }
    public async Task AddTeacherSubject(ControllerContext context)
    {
        context.Session.Teacher.Subject = context.Update.Message.Text;
        await _adminService.AddTeacher(new TeacherCreationDto()
        {
            FullName = context.Session.Teacher.FullName,
            PhoneNumber = context.Session.Teacher.PhoneNumber,
            Password = context.Session.Teacher.Password,
            Subject = context.Session.Teacher.Subject
        });
        await context.SendBoldTextMessage("Muvofaqqiyatli qo'shildi✅",context.TeachersDepartmentReplyKeyboardMarkup());
        context.Session.Teacher = null;
    }
    public async Task DeleteTeacherStart(ControllerContext context)
    {
        await context.SendBoldTextMessage("O'qituvchi ismi va familiyasini kiriting : \n Na'muna : G'olibjon Abdurasulov");
        context.Session.Action = nameof(DeleteTeacherFinish);
    } 
    
    public async Task DeleteTeacherFinish(ControllerContext context)
    {
       var res=await _adminService.DeleteTeacher(new Worker()
        {
            FullName = context.Message.Text
        });
       if (res is not null)
        await context.SendBoldTextMessage("Muvofaqqiyatli o'chirildi✅",context.TeachersDepartmentReplyKeyboardMarkup());
       
       else
        await context.SendBoldTextMessage("O'qituvchi topilmadi❌ \n Eslatma: O'qituvchi ism familiyasini kiritishga e'tiborli bo'ling",
            context.TeachersDepartmentReplyKeyboardMarkup());
    }

    public async Task AddTeachers(ControllerContext context)
    {
        await context.SendTextMessage("Excel fileni yuboring");
    }
}