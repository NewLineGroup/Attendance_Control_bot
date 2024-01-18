using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Infrastructure.Repositories;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class AdminDashboardController : ControllerBase
{
    private readonly WorkerRepository _repository;
    private readonly AuthService _authService;
    private readonly ParentService _parentService;
    private readonly LessonRepository _lessonRepository;

    public AdminDashboardController(ControllerManager.ControllerManager controllerManager,
        WorkerRepository clientDataService, AuthService authService,
        LessonRepository lessonRepository, ParentService parentService) : base(controllerManager)
    {
        _repository = clientDataService;
        _authService = authService;
        _lessonRepository = lessonRepository;
        _parentService = parentService;
    }

    public async Task Index(ControllerContext context)
    {
        Worker? worker = null;
        if (context.Session.Worker.Id != null)
            worker = await _repository.GetByIdAsync(context.Session.Worker.Id);

        if (worker is not null)
        {
            await context.SendBoldTextMessage(
                $"Salom, Xush kelibsiz {(string.IsNullOrEmpty(worker.FullName) ? context.Update.Message?.Chat.FirstName : worker.FullName)}!",
                replyMarkup: context.MakeAdminDashboardReplyKeyboardMarkup());
        }
        else
        {
            await context.SendErrorMessage("Foydalanuvchi ma'lumotlari topilmadi❌");
        }
    }

    public async Task LogOut(ControllerContext context)
    {
        await _authService.Logout(context.Session.Worker.Id);
        await context.TerminateSession();
        await context.SendBoldTextMessage("Logged out", replyMarkup: context.HomeControllerIndexButtons());
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
            case nameof(ReadLessonScheduleStart):
                await this.ReadLessonScheduleStart(context);
                break;
            case nameof(ReadLessonSchedule):
                await this.ReadLessonSchedule(context);
                break;
            case nameof(PostAnAdStart):
                await this.PostAnAdStart(context);
                break;
            case nameof(SendPostToAllParents):
                await this.SendPostToAllParents(context);
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
                case "Log out🚪":
                    context.Session.Action = nameof(LogOut);
                    break;
                case "Sozlamalar⚙️":
                    context.Session.Controller = nameof(SettingsController);
                    context.Session.Action = nameof(SettingsController.Index);
                    break;
                case "O'quvchilar bo'limi👨‍🎓👩‍🎓":
                    context.Session.Controller = nameof(StudentsDepartmentController);
                    context.Session.Action = nameof(StudentsDepartmentController.Index);
                    break;
                case "O'qituvchilar bo'limi👨‍🏫👩‍🏫":
                    context.Session.Controller = nameof(TeacherDepartmentController);
                    context.Session.Action = nameof(TeacherDepartmentController.Index);
                    break;
                case "Dars jadvalini kiritish":
                    context.Session.Action = nameof(ReadLessonScheduleStart);
                    break;
                case"E'lon joylash📤":
                    context.Session.Action = nameof(PostAnAdStart);
                    break;
                case"Ortga":
                    context.Session.Action = nameof(Index);
                    break;
            }
        }

        if (context.Message.Document != null && context.Session.Action == nameof(ReadLessonSchedule))
        {
            await this.ReadLessonSchedule(context);
        }
    }

    private async Task ReadLessonScheduleStart(ControllerContext context)
    {
        await context.SendTextMessage("Dars jadvalini excel formatida yuboring",replyMarkup:context.Back());
        context.Session.Action = nameof(ReadLessonSchedule);
    }
    public async Task ReadLessonSchedule(ControllerContext context)
    {
        var fileId = context.Message.Document.FileId;
        //var fileName = context.Message.Document.FileName;

        using (var fileStream = new MemoryStream())
        {
            var file = await _botClient.GetInfoAndDownloadFileAsync(fileId, fileStream);
            fileStream.Position = 0;

            var lessons = await ExcelService.ReadLessonSchedule(fileStream);
            if (lessons is not null)
            {
                var res = await _lessonRepository.AddAllAsync(lessons);
                await context.SendBoldTextMessage("Muvofaqqiyatli qo'shildi✅",
                    context.StudentsDepartmentReplyKeyboardMarkup());
                context.Session.Action = nameof(StudentsDepartmentController.Index);
            }
            else
                await context.SendBoldTextMessage(@"Siz yuborgan excel file mos emas! 
 Iltimos file bosh emasligi va ma'lumotlar berilgan na'muna kabi ekanligin tekshiring❌",
                    context.StudentsDepartmentReplyKeyboardMarkup());

        }
    }

    public async Task PostAnAdStart(ControllerContext context)
    {
        await context.SendBoldTextMessage(
            "E'lonni yuboring\nEslatma: Siz joylagan e'lon botdan foydalanuvchi barcha ota-onalarga yuboriladi",replyMarkup:context.Back());
        context.Session.Action = nameof(SendPostToAllParents);
    }

    public async Task SendPostToAllParents(ControllerContext context)
    {
        var parentsTelegramChatIds = _parentService.GetAllAsync().Result.Select(parent=>parent.TelegramChatId).Distinct().ToList();
        
        if (parentsTelegramChatIds is  null)
        {
            await context.SendBoldTextMessage("E'lon yuborish uchun ota-onalr mavjud emas");
            return;
        }
        
        string post = context.Message.Text;
        if (post is null)
        {
          await context.SendBoldTextMessage("E'lon faqat marndan iborat bo'lishi kerak");
          return;
        }

        foreach (long id in parentsTelegramChatIds)
        {
            Console.WriteLine(id);
        }
        foreach (long id in parentsTelegramChatIds)
        {
            if (_botClient != null) await _botClient.SendTextMessageAsync(id, post);
        }
        await context.SendBoldTextMessage("E'lon yuborildi",context.MakeAdminDashboardReplyKeyboardMarkup());
        context.Session.Action = nameof(Index);
    }
}
