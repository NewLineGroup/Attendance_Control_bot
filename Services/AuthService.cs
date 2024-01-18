using AttendanceControlBot.Domain.Dtos.AuthDto;
using AttendanceControlBot.Domain.Dtos.AuthDtos;
using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.Domain.Exceptions;
using AttendanceControlBot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace AttendanceControlBot.Services;

public class AuthService 
{
    private WorkerRepository _workerRepository; 

    public AuthService( WorkerRepository repository)
    {
        this._workerRepository = repository;
    }
   
    // public async Task RegisterUser(UserRegistrationDto userRegistration)
    // {
    //
    //     var oldUser  = await this.repository.GetAll()
    //         .FirstOrDefaultAsync(x => x.PhoneNumber == userRegistration.PhoneNumber);
    //
    //     if (oldUser is User)
    //         throw new Exception("User already exists");
    //     
    //     var insertedUser = await this.repository.AddAsync(new Worker
    //     {
    //         PhoneNumber = userRegistration.PhoneNumber,
    //         Password = userRegistration.Password,
    //         TelegramChatId = userRegistration.ChatId,
    //         Signed = false,
    //         LastLogindate = DateTime.Now,
    //         Role = Role.Teacher
    //     });
    //     
    //     if (insertedUser is null)
    //         throw new Exception("Unable to insert user");
    //     var client = new Client()
    //     {
    //         UserId = insertedUser.Id, // Updated line
    //         Status = ClientStatus.Enabled,
    //         IsPremium = false,
    //         UserName = string.Empty,
    //         Nickname = string.Empty
    //     };
    //
    //     var insertedClient = await this._clientDataService.AddAsync(client);
    //
    //     if (insertedClient is null)
    //         throw new Exception("Unable to add new client");
    // }

    public async Task<Worker?> Login(UserLoginDto user)
    {
        var userInfo = _workerRepository.GetAll()
            .FirstOrDefault(item => 
                item.Password == user.Password
                && item.PhoneNumber == user.Login);

        if (userInfo is null)
        {
            throw new UserException("Foydalanuvchi topilmadi");
        }

        if (userInfo is  Worker)
        {
             userInfo.Signed = true;
             userInfo.LastLoginDate = DateTime.Now;
        }

        if (userInfo.TelegramChatId==0)
            userInfo.TelegramChatId = user.TelegramChatId;

        await _workerRepository.UpdateAsync(userInfo);
        return userInfo;
    }

    public async Task Logout(long userId)
    {
        var user = await _workerRepository.GetByIdAsync(userId);
        if (user is null)
            throw new UserException("Foydalanuvchi topilmadi");

        user.Signed = false;
        await _workerRepository.UpdateAsync(user);
    }
}