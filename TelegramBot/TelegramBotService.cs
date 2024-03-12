using AttendanceControlBot.Configuration;
using AttendanceControlBot.Domain.Exceptions;
using AttendanceControlBot.Extensions;
using AttendanceControlBot.Infrastructure;
using AttendanceControlBot.Infrastructure.Repositories;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Context;
using AttendanceControlBot.TelegramBot.Controllers;
using AttendanceControlBot.TelegramBot.SimpleSessionManager;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace AttendanceControlBot.TelegramBot;

public class TelegramBotService
{

    public static TelegramBotClient? _client { get; set; }
    private List<Func<ControllerContext, CancellationToken, Task>> updateHandlers { get; set; }
    private DataContext _dataContext;

    private ControllerManager.ControllerManager ControllerManager;
    private WorkerRepository _workerRepository;
    private StudentRepository _studentRepository;
    private ParentRepository _parentRepository;
    private LessonRepository _lessonRepository;
    
    private AuthService _authService;
    private ParentService _parentService;
    private AdminService _adminService;
    private StudentService _studentService;
    private SessionManager SessionManager;
    private WorkerService WorkerService;
    private SettingsService SettingsService;

    public TelegramBotService()
    {
        _client = new TelegramBotClient(Settings.botToken);
        this.updateHandlers = new List<Func<ControllerContext, CancellationToken, Task>>();

        _dataContext = new DataContext();
        _studentRepository = new StudentRepository(_dataContext);
        _workerRepository = new WorkerRepository(_dataContext);
        _parentRepository = new ParentRepository(_dataContext);
        _lessonRepository = new LessonRepository(_dataContext);
        
        _adminService = new AdminService(_workerRepository, _studentRepository);
        _authService = new AuthService(repository: _workerRepository);
        _studentService = new StudentService(_studentRepository);
        _parentService = new ParentService(_parentRepository, _studentRepository);
        SessionManager = new SessionManager(_workerRepository);
        WorkerService = new WorkerService(_workerRepository);
        SettingsService = new SettingsService(WorkerService);
        ControllerManager =
            new ControllerManager.ControllerManager(_authService, SessionManager, repository: _workerRepository,
                _studentRepository, _adminService,_studentService,_parentService,_lessonRepository,SettingsService);
    }


    public async Task Start()
    {
        //Session handler
        this.updateHandlers.Add(async (context, token) =>
        {
            long? chatId = context.GetChatIdFromUpdate();

            if (chatId is null)
                throw new Exception("Chat id not found to find session");

            var session = await SessionManager.GetSessionByChatId(chatId.Value);
            context.Session = session;
            context.TerminateSession = async () => await this.SessionManager.TerminateSession(context.Session);
        });

        //Log handler
        this.updateHandlers.Add(async (context, token) =>
        {
            Console.WriteLine("Log -> {0} | {1} | {2}", DateTime.Now, context.Session.ChatId,
                context.Update.Message?.Text ?? context.Update.Message?.Caption);
        });


        this.updateHandlers.Add(async (context, token) =>
        {
            var signedUser = await _workerRepository
                .GetAll()
                .FirstOrDefaultAsync(user =>
                    user.Signed
                    && user.TelegramChatId == context.GetChatIdFromUpdate());
            if (signedUser is not null)
            {
                context.Session.Worker = signedUser;
                context.Session.Worker.TelegramChatId = signedUser.TelegramChatId;
            }
        });

        //Check for auth
        List<string> authRequiredControllers = new List<string>()
        {
            nameof(AdminDashboardController),
            nameof(StudentsDepartmentController),
            nameof(TeacherDepartmentController),
            nameof(TeacherDashboardController),
            //nameof(ParentsController),
        };

        this.updateHandlers.Add(async (context, token) =>
        {
            if (context.Session is not null)
            {
                if (context.Session.Worker.TelegramChatId > 0)
                {
                    string controller = context.Session.Controller ?? nameof(HomeController);
                    if (nameof(HomeController) == controller)
                    {
                        //|| controller == nameof(AuthController)
                        context.Session.Controller = nameof(HomeController);
                        context.Session.Action = nameof(HomeController);
                    }
                }
                else if (authRequiredControllers.Contains(context.Session.Controller))
                {
                    await context.SendErrorMessage("Unauthorized", 401);
                    context.Session.Controller = null;
                    context.Session.Action = null;
                }

                ;
            }
        });


        this.updateHandlers.Insert(this.updateHandlers.Count,
            async (context, token) => { await context.Forward(this.ControllerManager); });

        await StartReceiver();
    }


    private async Task StartReceiver()
    {
        var cancellationToken = new CancellationToken();
        var options = new ReceiverOptions();
        _client.StartReceiving(OnUpdate, ErrorMessage, options, CancellationToken.None);
        Console.WriteLine("{0} | Bot is starting...", DateTime.Now);
        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, args) => { cts.Cancel(); };

        while (!cts.IsCancellationRequested)
        {

        }
    }

    private async Task OnUpdate(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        ControllerContext context = new ControllerContext()
        {
            Update = update
        };

        try
        {
            foreach (var updateHandler in this.updateHandlers)
                await updateHandler(context, token);
        }
        catch (Exception e)
        {
            //context.Reset();

            string errorMessage = ("Handler Error: " + e.Message
                                                     + "\nInner exception message: "
                                                     + e.InnerException?.Message
                    // + "\nStack trace: " + e.StackTrace
                );
            Console.WriteLine(errorMessage + "\nStackTrace: " + e.StackTrace);
            if (e.GetType() == typeof(UserException))
            {
                context.Session.Controller = nameof(HomeController);
                context.Session.Action = nameof(HomeController.Index);
              await  context.SendErrorMessageToUser(chatId:context.Session.ChatId, e.Message);
            }
            else
             await context.SendErrorMessage(errorMessage, 500);
        }
    }


    private async Task ErrorMessage(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        // Handle any errors that occur during message processing here.
    }
}
    
   