using AttendanceControlBot.Domain.Entity;

namespace AttendanceControlBot.Services;

public class SettingsService
{
    private WorkerService WorkerService{ get; set;}

    public SettingsService(WorkerService workerService)
    {
        WorkerService = workerService;
    }

    public async Task<Worker> ChangePassword(Worker worker)
    {
        return await WorkerService.UpdateAsync(worker);
    }
    
    public async Task<Worker> ChangePhoneNumber(Worker worker)
    {
        return await WorkerService.UpdateAsync(worker);
    }
}