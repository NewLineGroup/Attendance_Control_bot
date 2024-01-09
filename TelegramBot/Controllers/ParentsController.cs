using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Infrastructure.Repositories;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot.Types.Enums;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class ParentsController : ControllerBase
{
    private readonly ParentService _parentService;
    public ParentsController(ControllerManager.ControllerManager controllerManager, ParentService parentService) : base(controllerManager)
    {
        _parentService = parentService;
        
    }

    public async Task Index(ControllerContext context)
    {
      await context.SendBoldTextMessage("Farzandingiz uchun berilgan tasdiqlash Id sini kiriting:",context.ParentControllerBackReplyKeyboardMarkup());
      context.Session.Action = nameof(CheckParentAuthId);
    }

    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            case nameof(CheckParentAuthId):
                await this.CheckParentAuthId(context);
                break;
            case nameof(WhenParentConfirmsStudent):
                await this.WhenParentConfirmsStudent(context);
                break;
            case nameof(Index):
                await this.Index(context);
                break;
            case nameof(LogOutStart):
                await this.LogOutStart(context);
                break; 
            case nameof(RegisterAnotherChildesStart):
                await this.RegisterAnotherChildesStart(context);
                break;
            case nameof(ViewAllMyChildes):
                await this.ViewAllMyChildes(context);
                break;
        }
    }

    protected override async Task HandleUpdate(ControllerContext context)
    {
        if (message!.Type == MessageType.Text)
        {
            var text = message.Text;
            if (text is not null)
                switch (text)
                {
                    case "Ortga":
                        context.Session.Controller = nameof(HomeController);
                        context.Session.Action = nameof(HomeController.Index);
                        break;
                     case "Hisobdan chiqish ":
                        context.Session.Action = nameof(LogOutStart);
                        break;
                    case "Baribir chiqishni xohlayman":
                        var res=_parentService.GetAllAsync().Result
                            .FirstOrDefault(p => p.TelegramChatId == context.Message.Chat.Id);
                        if (res is not null)
                        {
                            res.IsStopped = true;
                            _parentService.UpdateAsync(res);
                        }
                        context.Session.Controller=nameof(HomeController);
                        context.Session.Action = nameof(HomeController.Index);
                        break;
                    case "Bosh menyuga qaytish":
                        context.Session.Controller=nameof(ParentsController);
                        context.Session.Action = nameof(ParentsController.ParentsMenu);
                        break;
                    case "Boshqa farzandlar uchun ham ro'yxatdan o'tish":
                        context.Session.Action = nameof(RegisterAnotherChildesStart);
                        break; 
                    case "Farzandlar ro'yxatini ko'rish":
                        context.Session.Action = nameof(ViewAllMyChildes);
                        break; 
                    case "Xa✅":
                        context.Session.Action = nameof(this.WhenParentConfirmsStudent);
                        break; 
                    case "Yo'q❌":
                        context.Session.Action = nameof(ParentsController.Index);
                        break;
                }
        }
    }
    
    protected async Task CheckParentAuthId(ControllerContext context)
    {
        long id;
        if (long.TryParse(context.Message.Text, out id))
        {
            var student=await _parentService.CheckParentAuthId(id);
            if (student is not null)
            {
                var res= _parentService.GetAllAsync().Result.FirstOrDefault(p => p.ChildId == student.Id);
                if (res is not null)
                {
                    context.SendBoldTextMessage("Bu o'quvchi uchun ro'yxatdan o'tilgan");
                    context.Session.Action = nameof(Index);
                    return;
                }

                context.Session.Student = student;
                context.SendBoldTextMessage(
                    $@"Farzandingiz {student.FirstName} {student.LastName} ekanligini tasdiqlaysizmi?",context.ConfirmationOfParentageReplayKeyboardMarkup());
                context.Session.Action = nameof(WhenParentConfirmsStudent);
            }

        }
        
    }

    private async Task ParentsMenu(ControllerContext context)
    {
        await context.SendBoldTextMessage("Farzandingiz uchun berilgan tasdiqlash Id sini kiriting:",context.ParentMenuReplyKeyboardMarkup());
    }

    private async Task ViewAllMyChildes(ControllerContext context)
    {
        string textMessage = $"Farzandingiz: ";
        var students =await _parentService.GetParentAllChildes(context.Update.Message.Chat.Id);
        Console.WriteLine(students.FirstOrDefault().FirstName);
        foreach (Student student in students)
        {
            textMessage += $"\n{student.FirstName} {student.LastName}   sinfi: {student.ClassNumber}";
        }

        await context.SendBoldTextMessage(textMessage);
        context.Session.Action = nameof(ParentsMenu);
    }

    private async Task RegisterAnotherChildesStart(ControllerContext context)
    {
        await context.SendBoldTextMessage("Farzandingiz uchun berilgan tasdiqlash Id sini kiriting:",context.ParentControllerBackToMainMenuReplyKeyboardMarkup());
        context.Session.Action = nameof(CheckParentAuthId);
    }
    
    private async Task LogOutStart(ControllerContext context)
    {
        context.SendBoldTextMessage(@"Rostdan ham chiqishni istaysizi?
Eslatma: Agar hisobingizdan chiqadigan bo'lsangiz bildirishnomalarni ola olmaysiz, ya'ni farzandingiz haqidagi xabarlarni ola olmaysiz",context.ParentConfirmTheLogOutReplayKeyboardMarkup());
    }
    
    protected async Task WhenParentConfirmsStudent(ControllerContext context)
    {
        Parent parent = new Parent
        {
            ChildId =context.Session.Student.Id,
            AuthId = context.Session.Student.ParentAuthId,
            TelegramChatId = context.Message.Chat.Id 
        }; 
        await _parentService.CreateAsync(parent); 
        await context.SendBoldTextMessage($@"Siz muvofaqqiyatli ro'yxatdan o'tdingiz✅
Farzandingiz: {context.Session.Student.FirstName} {context.Session.Student.LastName}",replyMarkup: context.ParentMenuReplyKeyboardMarkup());
        context.Session.Action = nameof(ParentsMenu);
        context.Session.Student = null;
    }
    
    // protected async Task WhenParentsDoNotConfirmStudent(ControllerContext context)
    // {
    //     long id;
    //
    //     if (long.TryParse(context.Message.Text, out id))
    //     {
    //         var student=await _parentService.CheckParentAuthId(id);
    //         if (student is not null)
    //         {
    //             context.SendBoldTextMessage(
    //                 $@"Farzandingiz {student.FirstName} {student.LastName} ekanligini tasdiqlaysizmi?",context.ConfirmationOfParentageReplayKeyboardMarkup());
    //         }
    //
    //     }
    // }
}

