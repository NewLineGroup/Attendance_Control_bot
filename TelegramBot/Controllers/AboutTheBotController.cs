using AttendanceControlBot.Extensions;
using AttendanceControlBot.TelegramBot.Context;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class AboutTheBotController : ControllerBase
{
    public AboutTheBotController(ControllerManager.ControllerManager controllerManager) : base(controllerManager)
    {
    }

    public async Task Index(ControllerContext context)
    {
        string message = $@"Botning maqsadi darslarga o'z vaqtida qatnashishmagan o'quvchilar uchun mas'ul shaxslarga xabar yuborish.
Savol, taklif va tushunmovchiliklar uchun: @G_Abdurasulov
Yaratuvchi: @G_Abdurasulov";
       await context.SendTextMessage(message,context.HomeControllerIndexButtons());
       context.Session.Controller = nameof(HomeController);
       context.Session.Action = nameof(HomeController.Index);
    }

    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            case nameof(Index):
                await this.Index(context);
                break;
        }
    }

    protected override async Task HandleUpdate(ControllerContext context)
    {
    }
}