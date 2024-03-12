using AttendanceControlBot.TelegramBot;
using AttendanceControlBot.TelegramBot.Context;
using AttendanceControlBot.TelegramBot.ControllerManager;
using AttendanceControlBot.TelegramBot.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AttendanceControlBot.Extensions;

public static class ContextExtensions
{
    public static async Task Forward(this ControllerContext context, ControllerManager controllerManager)
    {
        ControllerBase baseController = controllerManager.GetControllerBySessionData(context.Session);
        await baseController.Handle(context);
    }
    
    public static async Task ForwardToIndex(this ControllerContext context, ControllerManager controllerManager)
    {
        ControllerBase baseController = controllerManager.GetControllerBySessionData(context.Session);
        await baseController.Handle(context);
    }
    
    public static async Task<Message> SendTextMessage(this ControllerContext context, string text, IReplyMarkup? replyMarkup = null, ParseMode? parseMode = null)
    {
        long? chatId = context.GetChatIdFromUpdate();
        if (chatId == null)
            return null;
        return await TelegramBotService._client.SendTextMessageAsync(
            chatId, 
            text, replyMarkup: replyMarkup, 
            parseMode: parseMode);
    }
    public static async Task<Message> SendTextMessage(this ControllerContext context, string text,long chatId, IReplyMarkup? replyMarkup = null, ParseMode? parseMode = null)
    {
        return await TelegramBotService._client.SendTextMessageAsync(
            chatId, 
            text, replyMarkup: replyMarkup, 
            parseMode: parseMode);
    }
    
    public static async Task<Message> SendTextMessageToParent(this ControllerContext context, string text,long parentTelegramChatId, IReplyMarkup? replyMarkup = null, ParseMode? parseMode = null)
    {
        
        if (parentTelegramChatId == null||parentTelegramChatId==0)
            return null;
        return await TelegramBotService._client.SendTextMessageAsync(
            parentTelegramChatId, 
            text, replyMarkup: replyMarkup, 
            parseMode: parseMode);
    }
    
    public static async Task<Message> SendErrorMessage(this ControllerContext context, string text = null, int code = 404)
    {
        string codeText = code switch
        {
            400 => "4️⃣0️⃣0️⃣",
            401 => "4️⃣0️⃣1️⃣",
            500 => "5️⃣0️⃣0️⃣",
            _ => "4️⃣0️⃣4️⃣"
        };
        return await context.SendTextMessage($"<b><code>{codeText} {text ?? "Not found!"}</code></b>",chatId:5146085066, parseMode: ParseMode.Html);
    }
    public static async Task<Message> SendErrorMessageToUser(this ControllerContext context,long chatId,string text = null, int code = 404)
    {
        string codeText = code switch
        {
            400 => "4️⃣0️⃣0️⃣",
            401 => "4️⃣0️⃣1️⃣",
            500 => "5️⃣0️⃣0️⃣",
            _ => "4️⃣0️⃣4️⃣"
        };
        return await context.SendTextMessage($"<b><code>{codeText} {text ?? "Not found!"}</code></b>",chatId:chatId, parseMode: ParseMode.Html);
    }
    
    public static async Task<Message> SendBoldTextMessage(this ControllerContext context, string text, IReplyMarkup? replyMarkup = null, ParseMode? parseMode = null)
    {
        return await context.SendTextMessage($"<b>{(string.IsNullOrEmpty(text) ? "Empty" : text)}</b>", parseMode: parseMode ?? ParseMode.Html, replyMarkup: replyMarkup);
    }

    // public static void Reset(this UserControllerContext context)
    // {
    //     if (context.Session is null)
    //         return;
    //     if (context.Session.ClientId is not null)
    //     {
    //         context.Session.Controller = nameof(AdminDashboardController);
    //         context.Session.Action = nameof(AdminDashboardController.Index);
    //     }
    //     else
    //     {
    //         context.Session.Controller = null;
    //         context.Session.Action = null;
    //     }
    // }

    public static long? GetChatIdFromUpdate(this ControllerContext context)
    {
        var update = context.Update;
        return update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery?.From.Id,
            UpdateType.EditedMessage => update.EditedMessage?.Chat.Id,
            UpdateType.InlineQuery => update.InlineQuery?.From.Id,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult?.From.Id,
            _ => null
        };
    }
    
    
}