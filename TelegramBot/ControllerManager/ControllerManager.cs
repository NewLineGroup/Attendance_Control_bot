using AttendanceControlBot.Domain.SessionModels;
using AttendanceControlBot.Infrastructure;
using AttendanceControlBot.Infrastructure.Repositories;
using AttendanceControlBot.Services;
using AttendanceControlBot.TelegramBot.Controllers;
using AttendanceControlBot.TelegramBot.SimpleSessionManager;

namespace AttendanceControlBot.TelegramBot.ControllerManager;

public class ControllerManager
{
    private readonly SettingsController _settingsController;
    private readonly AuthController _authController;
    private readonly HomeController _homeController;
    private readonly AdminDashboardController _adminDashboardController;
    private readonly StudentsDepartmentController studentsDepartmentController;
    private readonly TeacherDepartmentController teacherDepartmentController;
    private readonly TeacherDashboardController _teacherDashboardController;
    private readonly ParentsController _parentsController;
    private readonly AboutTheBotController _aboutTheBotController;
    private readonly SessionManager _sessionManager;
    private DataContext _dataContext;

    //private readonly SettingsController _settingsController;

    private WorkerRepository workerRepository;
    private StudentRepository StudentRepository;
    private LessonRepository LessonRepository;

    private AdminService AdminService;
    private ParentService _parentService;
    private StudentService studentService;
    private SettingsService SettingsService;
    
    public ControllerManager(AuthService authService,
        SessionManager sessionManager, WorkerRepository repository,StudentRepository studentRepository, AdminService adminService, StudentService studentService, ParentService parentService, LessonRepository lessonRepository, SettingsService settingsService)
    {
        
        _sessionManager = sessionManager;
        workerRepository = repository;
        StudentRepository = studentRepository;
        AdminService = adminService;
        this.studentService = studentService;
        _parentService = parentService;
        LessonRepository = lessonRepository;
        SettingsService = settingsService;
        _parentsController = new ParentsController(this,_parentService);
        _aboutTheBotController = new AboutTheBotController(this);
        _teacherDashboardController = new TeacherDashboardController(this,workerRepository,authService,this.studentService,_parentService);
        this.teacherDepartmentController = new TeacherDepartmentController(this, adminService);
        this.studentsDepartmentController = new StudentsDepartmentController(this,adminService);
        this._homeController = new HomeController(this,_parentService);
        this._authController = new AuthController(authService, this);
        this._adminDashboardController = new AdminDashboardController(this, repository, authService,LessonRepository,_parentService);
        this._settingsController = new SettingsController(this,settingsService);
        this._dataContext = new DataContext();
        //this._settingsController = new SettingsController(this,clientDataService, boardService);
        //_clientInfoController = new ClientInfoController(this,clientDataService,_clientService);


        // this._authController = new AuthController(botClient, new AuthService(dataService));
    }

    // public ControllerManager(DataContext dataContext, StudentsDepartmentController studentsDepartmentController)
    // {
    //     _dataContext = dataContext;
    //     this.studentsDepartmentController = studentsDepartmentController;
    // }

    public ControllerBase GetControllerBySessionData(Session session)
    {
        switch (session.Controller)
        {
            case nameof(HomeController):
                return this._homeController;
            case nameof(AuthController):
                return this._authController;
            case nameof(AdminDashboardController):
                return this._adminDashboardController;
            case nameof(StudentsDepartmentController):
                return this.studentsDepartmentController;
            case nameof(TeacherDepartmentController):
                return this.teacherDepartmentController;
            case nameof(TeacherDashboardController):
                return this._teacherDashboardController; 
            case nameof(ParentsController):
               return this._parentsController;
            case nameof(AboutTheBotController):
                return this._aboutTheBotController;
            case nameof(SettingsController):
                return this._settingsController;
        }
        return this.DefaultController;
    }

    public ControllerBase DefaultController => this._homeController;
}