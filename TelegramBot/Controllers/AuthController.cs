using AttendanceControlBot.Domain.Dtos.AuthDtos;
using AttendanceControlBot.Domain.Enums;
using AttendanceControlBot.Domain.SessionModels;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;

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
            await context.SendErrorMessage("Yaroqsiz telefon raqami!", 400);
            return;
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
            //context.Session.Worker.TelegramChatId = context.Update.Message.Chat.Id;
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
                // default:
                //     context.Session.Controller = nameof(TeacherDepartmentController);
                //     context.Session.Action = nameof(TeacherDepartmentController.Index);
                //     break;
            }
           

            await context.Forward(this._controllerManager);
            return;
        }
        else
            await context.SendBoldTextMessage("User not found❌");

        context.Session.Controller = null;
        context.Session.Action = null;

        await context.Forward(this._controllerManager);
    }

    // public async Task RegistrationStart(UserControllerContext context)
    // {
    //     context.Session.RegistrationModel = new UserRegistrationModel();
    //     await context.SendTextMessage("Enter your phone number :", context.RequesPhoheNumberReplyKeyboardMarkup());
    //     context.Session.Action = nameof(RegistrationPhoneNumber);
    // }

    // public async Task RegistrationPhoneNumber(UserControllerContext context)
    // {
    //     var phoneNumber = context.Message?.Contact?.PhoneNumber;
    //     if (phoneNumber is null)
    //     {
    //         await context.SendErrorMessage("Wrong phone number!", 400);
    //         return;
    //     }
    //     
    //     if (!phoneNumber.StartsWith("+"))
    //         phoneNumber = "+" + phoneNumber;
    //     
    //     context.Session.RegistrationModel.PhoneNumber = phoneNumber;
    //     await context.SendTextMessage("Please Enter your password: ");
    //     context.Session.Action = nameof(RegistrationPassword);
    // }

    // public async Task RegistrationPassword(UserControllerContext context)
    // {
    //     context.Session.RegistrationModel.Password = context.Update.Message.Text;
    //     context.Session.RegistrationModel.ChatId = context.Session.ChatId;
    //
    //     await _authService.RegisterUser(context.Session.RegistrationModel);
    //
    //     await context.SendBoldTextMessage("You Succesfully registired. Please re-sign in✅");
    //
    //     context.Session.Controller = null;
    //     context.Session.Action = null;
    //
    //     await context.Forward(this._controllerManager);
    // }


    protected override async Task HandleAction(ControllerContext context)
    {
        switch (context.Session.Action)
        {
            // case nameof(RegistrationStart):
            // {
            //     await RegistrationStart(context);
            //     break;
            // }
            // case nameof(RegistrationPhoneNumber):
            // {
            //     await RegistrationPhoneNumber(context);
            //
            //     break;
            // }
            // case nameof(RegistrationPassword):
            // {
            //     await RegistrationPassword(context);
            //
            //
            //     break;
            // }
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

    protected override Task HandleUpdate(ControllerContext context)
    {
        return Task.CompletedTask;
    }
}