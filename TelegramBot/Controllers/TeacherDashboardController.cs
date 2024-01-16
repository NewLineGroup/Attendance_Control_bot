using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Domain.Exceptions;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Infrastructure.Repositories;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class TeacherDashboardController : ControllerBase
{
    private WorkerRepository _workerRepository;
    private ParentService _parentService;
    private AuthService _authService;
    private StudentService _studentService;

    public TeacherDashboardController(ControllerManager.ControllerManager controllerManager,
        WorkerRepository workerRepository, AuthService authService, StudentService studentService, ParentService parentService) : base(controllerManager)
    {
        _workerRepository = workerRepository;
        _authService = authService;
        _studentService = studentService;
        _parentService = parentService;
    }

    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            case nameof(Index):
                await this.Index(context);
                break;
            case nameof(LogOut):
                await this.LogOut(context);
                break;
            case nameof(HomeworkAssignment):
                await this.HomeworkAssignment(context);
                break;
            case nameof(UncompletedHomeworkMessage):
                await this.UncompletedHomeworkMessage(context);
                break;
            case nameof(ToAttend):
                await this.ToAttend(context);
                break;
            case nameof(HandleStudentNamesCallBackQueryToAttendance):
                await this.HandleStudentNamesCallBackQueryToAttendance(context);
                break;
            case nameof(HandleClassNumberCallBackQuery):
                await this.HandleClassNumberCallBackQuery(context);
                break;
            case nameof(GetClassNumber):
                await this.GetClassNumber(context);
                break;
            case nameof(GetStudentNameToAttendance):
                await this.GetStudentNameToAttendance(context);
                break;
            case nameof(MessageAboutNonAttendance):
                await this.MessageAboutNonAttendance(context);
                break;
            case nameof(GetStudentNameToMessageAboutBeingLateToClass):
                await this.GetStudentNameToMessageAboutBeingLateToClass(context);
                break;
            case nameof(MessageAboutBeingLateToClass):
                await this.MessageAboutBeingLateToClass(context);
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
                case "Davomat qilish📝":
                    context.Session.Action = nameof(GetClassNumber);
                    break;
                case "Uyga vazifa berish📬":
                    context.Session.Action = nameof(HomeworkAssignment);
                    break;
                case "Uyga vazifa bajarilmaganligi xabari⚠️":
                    context.Session.Action = nameof(UncompletedHomeworkMessage);
                    break;
                case "Darsga qatnashmaganlik xabari⛔️":
                    context.Session.Action = nameof(GetStudentNameToAttendance);
                    break;
                case "Darsga kechikib kirganligi  xabari⚠️":
                    context.Session.Action = nameof(GetStudentNameToMessageAboutBeingLateToClass);
                    break;
                case "Sozlamalar⚙️":
                    context.Session.Controller = nameof(SettingsController);
                    context.Session.Action = nameof(SettingsController.Index);
                    break;
                case "Ortga":
                    context.Session.Controller = nameof(TeacherDashboardController);
                    context.Session.Action = nameof(TeacherDashboardController.Index);
                    break;
            }
        }
        if (context.Update.Type == UpdateType.CallbackQuery)
        {
             string action = context.Session.Action;
            
            //var res=await CheckCallBackQueryTypeIsStudentName(context.Update.CallbackQuery.Data);
            // context.Session.Action = res ? nameof(HandleStudentNamesCallBackQuery) : nameof(HandleClassNumbersCallBackQuery);
            int id = context.Update.CallbackQuery.Message.MessageId;
            switch (action)
            {
                case nameof(HandleClassNumberCallBackQuery):
                   await HandleClassNumberCallBackQuery(context);
                  await _botClient.DeleteMessageAsync(context.Session.ChatId, id);
                   break; 
                case nameof(HandleStudentNamesCallBackQueryToAttendance):
                   await HandleStudentNamesCallBackQueryToAttendance(context);
                  await _botClient.DeleteMessageAsync(context.Session.ChatId, id);
                   break;
                case nameof(HandleStudentNamesCallBackQueryToMessageAboutBeingLateToClass):
                   await HandleStudentNamesCallBackQueryToMessageAboutBeingLateToClass(context);
                  await _botClient.DeleteMessageAsync(context.Session.ChatId, id);
                   break;
            }
        }
    }

    public async Task Index(ControllerContext context)
    {
        Worker? worker = null;
        if (context.Session.Worker.Id != null)
            worker = await _workerRepository.GetByIdAsync(context.Session.Worker.Id);

        if (worker is not null)
        {
            await context.SendBoldTextMessage(
                $"Salom, Xush kelibsiz {(string.IsNullOrEmpty(worker.FullName) ? context.Update.Message?.Chat.FirstName : worker.FullName)}!",
                replyMarkup: context.MakeTeacherDashboardToSingleClassReplyKeyboardMarkup());
        }
        else
        {
            await context.SendErrorMessage("Foydalanuvchi ma'lumotlari topilmadi❌");
        }

    }

 
 
    public async Task HandleStudentNamesCallBackQueryToAttendance(ControllerContext context)
    {
        var query = context.Update.CallbackQuery;

        if (!long.TryParse(query.Data, out long studentId))
        {
            if (query.Data == "Ortga")
            {
                context.Session.Action = nameof(ToAttend);
                return;
            }
            else
                throw new UserException("Bunday o'quvchi mavjud emas");
        }

        var student = await _studentService.GetByIdAsync(studentId);
        if (student is not null)
        {
            context.Session.Student = student;
            context.Session.Action = nameof(MessageAboutNonAttendance);
        }
    }

    public async Task HandleStudentNamesCallBackQueryToMessageAboutBeingLateToClass(ControllerContext context)
    {
        var query = context.Update.CallbackQuery;

        if (!long.TryParse(query.Data, out long studentId))
        { if (query.Data == "Ortga")
            {
                context.Session.Action = nameof(ToAttend);
                return;
            }
            else
                throw new UserException("Bunday o'quvchi mavjud emas");
        }

        var student = await _studentService.GetByIdAsync(studentId);
        if (student is not null)
        {
            context.Session.Student = student;
            context.Session.Action = nameof(MessageAboutBeingLateToClass);
        }
    }

    public async Task HandleClassNumberCallBackQuery(ControllerContext context)
    {
       
        var @class = await _studentService.GetClassNumber(context.Update.CallbackQuery.Data);
        if (@class is not null)
        {
            context.Session.ClassNumber = @class;
            context.Session.Action = nameof(ToAttend);
        }
    }

    
    
    public async Task GetStudentNameToAttendance(ControllerContext context)
    {
        var students =await _studentService.GetStudentsByClassNumber(context.Session.ClassNumber);
        await context.SendBoldTextMessage("O'quvchini tanlang: ", replyMarkup: context.MakeStudentsInlineKeyboardMarkup(students));
        context.Session.Action = nameof(HandleStudentNamesCallBackQueryToAttendance);
    }
    
    public async Task GetStudentNameToMessageAboutBeingLateToClass(ControllerContext context)
    {
        var students =await _studentService.GetStudentsByClassNumber(context.Session.ClassNumber);
        await context.SendBoldTextMessage("O'quvchini tanlang: ", replyMarkup: context.MakeStudentsInlineKeyboardMarkup(students));
        context.Session.Action = nameof(HandleStudentNamesCallBackQueryToMessageAboutBeingLateToClass);
    }
    
    public async Task GetClassNumber(ControllerContext context)
    {
        var classes =await _studentService.GetAllClassNumbers();
        await context.SendBoldTextMessage("Sinfni tanlang: ", replyMarkup: context.MakeGetAllClassNumbersInlineReplyMarkup(classes));
        context.Session.Action = nameof(HandleClassNumberCallBackQuery);
    }


    private async Task SendAttendanceMessageToParent(ControllerContext context, Student student)
    {
        string studentFullName = student.FirstName + " " + student.LastName;
        string message =
            await GenerateMessageToAttendance(studentFullName);
        var parent = await _parentService.GetParentByStudentId(student.Id);
        if (parent is not null && !parent.IsStopped)
            await context.SendTextMessageToParent(message, parent.TelegramChatId);
        else
        {
            await context.SendTextMessage(
                "Xabar yuborilmadi.Bu o'quvchi uchun ro'yxatdan o'tilmagan yoki bot stop qilingan");
        }
    }
    private async Task SendMessageAboutBeingLateToClassToParent(ControllerContext context, Student student)
    {
        string studentFullName = student.FirstName + " " + student.LastName;
        string message =
            await GenerateMessageToMessageAboutBeingLateToClass(studentFullName);
        var parent = await _parentService.GetParentByStudentId(student.Id);
        if (parent is not null && !parent.IsStopped)
            await context.SendTextMessageToParent(message, parent.TelegramChatId);
        else
        {
            await context.SendTextMessage(
                "Xabar yuborilmadi.Bu o'quvchi uchun ro'yxatdan o'tilmagan yoki bot stop qilingan");
        }
    }

    private async Task SendUncompletedHomeworkMessageToParent(ControllerContext context, Student student)
    {
        string studentFullName = student.FirstName + " " + student.LastName;
        string message =
            await GenerateMessageUncompletedHomework(studentFullName, context.Session.Teacher.Subject, context.Session.LessonTime);
        var parent =await _parentService.GetParentByStudentId(student.Id);
        if (parent is not null)
         await context.SendTextMessageToParent(message,parent.TelegramChatId);
    }

    

    public async Task UncompletedHomeworkMessage(ControllerContext context)
    {
        await context.SendTextMessage("Tanlang: ",
            replyMarkup: context.MakeTeacherAttendanceSingleClassReplyKeyboardMarkup());
    } 
    public async Task MessageAboutNonAttendance(ControllerContext context)
    {
        await  SendAttendanceMessageToParent(context, context.Session.Student);
        var students =await _studentService.GetStudentsByClassNumber(context.Session.ClassNumber);
        await context.SendBoldTextMessage("O'quvchini tanlang: ", replyMarkup: context.MakeStudentsInlineKeyboardMarkup(students));
        // await context.SendBoldTextMessage("Xabar yuborildi");
        context.Session.Action = nameof(HandleStudentNamesCallBackQueryToAttendance);
    } 
    
    public async Task MessageAboutBeingLateToClass (ControllerContext context)
    {
        await  SendMessageAboutBeingLateToClassToParent(context, context.Session.Student);
        var students =await _studentService.GetStudentsByClassNumber(context.Session.ClassNumber);
        await context.SendBoldTextMessage("O'quvchini tanlang: ", replyMarkup: context.MakeStudentsInlineKeyboardMarkup(students));
        context.Session.Action = nameof(HandleStudentNamesCallBackQueryToMessageAboutBeingLateToClass);
    } 
    
    public async Task HomeworkAssignment(ControllerContext context)
    {
        
    }
    
    public async Task ToAttend(ControllerContext context)
    {
        await context.SendTextMessage("Tanlang: ",
            replyMarkup: context.MakeTeacherAttendanceSingleClassReplyKeyboardMarkup());
    }
    
    
    public async Task LogOut(ControllerContext context)
    {
        await _authService.Logout(context.Session.Worker.Id);
        await context.TerminateSession();
        await context.SendBoldTextMessage("Logged out", replyMarkup: context.HomeControllerIndexButtons());
    }

    public async Task<string> GenerateMessageToAttendance(string studentFullName)
    {
        string message = $"O'quvchi {studentFullName} ayni vaqtda darsga qatnashmayapti";
        return message;
    }
    public async Task<string> GenerateMessageToMessageAboutBeingLateToClass(string studentFullName)
    {
        string message = $"O'quvchi {studentFullName} ayni vaqtdagi darsga Kechikib kirdi";
        return message;
    }
    public async Task<string> GenerateMessageUncompletedHomework(string studentFullName,string subject,byte lessonTime)
    {
        string message = $"O'quvchi {studentFullName} {lessonTime}-soatdagi {subject} darsining uyga vazifasini bajarmadi❌";
        return message;
    }


    public async Task<bool> CheckCallBackQueryTypeIsStudentName(string callBackData)
    {
        if (callBackData.Length > 3)
            return true;
        return false;
    }
}