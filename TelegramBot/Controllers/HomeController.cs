using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot.Types.Enums;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class HomeController : ControllerBase
{
    private ParentService _parentService;
    public HomeController(ControllerManager.ControllerManager controllerManager, ParentService parentService) : base(controllerManager)
    {
        _parentService = parentService;
    }
    public async Task Index(ControllerContext context)
    {
        await context.SendBoldTextMessage(
            "Assalomu alaykum 40-maktabning davomatni nazorat qiluvchi botga xush kelibsiz!!!",
            replyMarkup: context.HomeControllerIndexButtons());
    }
    
    protected override async Task HandleAction(ControllerContext context)
    {
        // if (context.Session.Action == nameof(this.Index))
        //     await this.Index(context);
        await this.Index(context);
    }

    protected override async Task HandleUpdate(ControllerContext context)
    {
        //Check commands
        if (message!.Type == MessageType.Text)
        {
            var text = message.Text;
            if (text is not null)
                switch (text)
                {
                    case "/start":
                        context.Session.Action = nameof(Index);
                        break;
                    case "Login":
                        context.Session.Controller = nameof(AuthController);
                        context.Session.Action = nameof(AuthController.LoginUserStart);
                        break;
                    case "Ota-ona sifatida tashrif buyurish":
                        var res = await CheckParentTelegramChatId(context.Message.Chat.Id);
                        if (res is not null)
                        {
                          context.Session.Controller = nameof(ParentsController);
                          context.Session.Action = nameof(ParentsController.ParentsMenu);
                        }
                        else
                        {
                            context.Session.Controller = nameof(ParentsController);
                            context.Session.Action = nameof(ParentsController.Index);
                        }
                        break;
                    case "Bot haqida":
                        context.Session.Controller = nameof(AboutTheBotController);
                        context.Session.Action = nameof(AboutTheBotController.Index);
                        break;
                }
        }
    }

    private async Task<Parent> CheckParentTelegramChatId(long id)
    {
        var parents=  await _parentService.GetAllAsync();
        return parents.FirstOrDefault(parent => parent.TelegramChatId == id);
    }
}