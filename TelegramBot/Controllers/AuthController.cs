using System.Text.RegularExpressions;
using AttendanceControlBot.Domain.Dtos.AuthDtos;
using AttendanceControlBot.Domain.Enums;
using AttendanceControlBot.Domain.SessionModels;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot.Types.Enums;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService, ControllerManager.ControllerManager controllerManager) : base(
        controllerManager)
    {
        _authService = authService;
    }

    public async Task LoginUserStart(ControllerContext context)
    {
        await context.SendBoldTextMessage("Iltimos telefon raqamingizni kiriting : ",
            context.RequestPhoneNumberReplyKeyboardMarkup());

        context.Session.LoginData = new LoginSessionModel();
        context.Session.Action = nameof(LoginUserLogin);
    }

    private async Task LoginUserLogin(ControllerContext context)
    {
        var login = context.Message?.Contact?.PhoneNumber;
        if (login is null)
        {
            login = context.Message.Text;
            string regexPattern = @"^\+\d{12}$";
            Regex regex = new Regex(regexPattern);


            if (!regex.IsMatch(login))
            {
                await context.SendErrorMessage("Yaroqsiz telefon raqami!", 400);
                return;
            }
        }

        if (!login.StartsWith("+"))
            login = "+" + login;
        context.Session.LoginData.Login = login;
        await context.SendBoldTextMessage("Parolingizni kiriting: ");
        context.Session.Action = nameof(LoginUserPassword);
    }

    private async Task LoginUserPassword(ControllerContext context)
    {
        var password = context.Update.Message.Text;
        var worker = await _authService.Login(new UserLoginDto()
        {
            Login = context.Session.LoginData.Login,
            Password = password
        });
        if (worker.TelegramChatId == 0)
            worker.TelegramChatId = context.Update.Message.Chat.Id;
            
        
        context.Session.LoginData = null;
        if (worker is not null)
        {
            context.Session.Worker.TelegramChatId = context.Update.Message.Chat.Id;
            context.Session.Worker.Id = worker.Id;
           
            switch (worker.Role)
            {
                case Role.Teacher:
                    context.Session.Controller = nameof(TeacherDashboardController);
                    context.Session.Action = nameof(TeacherDashboardController.Index);
                    break;
                case Role.Admin:
                    context.Session.Controller = nameof(AdminDashboardController);
                    context.Session.Action = nameof(AdminDashboardController.Index);
                    break;
                default:
                    context.Session.Controller = nameof(HomeController);
                    context.Session.Action = nameof(HomeController.Index);
                    break;
            }
           

            await context.Forward(this._controllerManager);
            return;
        } 
        await context.SendBoldTextMessage("Foydalanuvchi topilmadi❌");

        context.Session.Controller = null;
        context.Session.Action = null;

        await context.Forward(this._controllerManager);
    }

   


    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            case nameof(LoginUserStart):
            {
                await LoginUserStart(context);
                break;
            }
            case nameof(LoginUserLogin):
            {
                await LoginUserLogin(context);
                break;
            }
            case nameof(LoginUserPassword):
            {
                await LoginUserPassword(context);
                break;
            }
        }

        return;
    }

    protected override async Task HandleUpdate(ControllerContext context)
    {
        if (message!.Type == MessageType.Text)
        {
            var text = message.Text;
            if (text is not null)
                switch (text)
                {
                    case "/start":
                        context.Session.Controller = nameof(HomeController);
                        context.Session.Action = nameof(HomeController.Index);
                        break;
                }
        }
    }
}