using AttendanceControlBot.TelegramBot.Context;

namespace AttendanceControlBot.TelegramBot.Controllers;

public class SettingsController : ControllerBase
{
    

    public SettingsController(ControllerManager.ControllerManager controllerManager) : base(controllerManager)
    {
    }

    protected override async Task HandleAction(ControllerContext context)
    {
        
    }

    public async Task Index(ControllerContext context)
    {
        
    }

    protected override Task HandleUpdate(ControllerContext context)
    {
        throw new NotImplementedException();
    }
}