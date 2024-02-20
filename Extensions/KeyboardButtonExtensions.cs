using AttendanceControlBot.Domain.Entity;
using AttendanceControlBot.TelegramBot.Context;
using Telegram.Bot.Types.ReplyMarkups;

namespace AttendanceControlBot.Extensions;

public static class KeyboardButtonExtensions
{
    public static ReplyKeyboardMarkup MakeAdminDashboardReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("O'quvchilar bo'limi👨‍🎓👩‍🎓"),
                new KeyboardButton("O'qituvchilar bo'limi👨‍🏫👩‍🏫"),
            },
            new List<KeyboardButton>()
            {   
                new KeyboardButton("E'lon joylash📤"),
                new KeyboardButton("Sozlamalar⚙️"),
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Log out🚪")
            }
        };

        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    
    public static ReplyKeyboardMarkup MakeTeacherDashboardToSingleClassReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Davomat qilish📝"),
                new KeyboardButton("Sozlamalar⚙️"),
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Log out🚪") 
            }
        };

        return MakeDefaultReplyKeyboardMarkup(buttons);
    }

    public static ReplyKeyboardMarkup MakeSettingsControllerIndexButtonsReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Parolni o'zgartirish")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Ortga") 
            }
        };

        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
     public static ReplyKeyboardMarkup MakeDeleteClassConfirmation(this ControllerContext context)
     {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {                
                new KeyboardButton("Xa✅"),
                new KeyboardButton("Yo'q❌")
            }
        };
        return MakeDefaultReplyKeyboardMarkup(buttons);
     }
     public static ReplyKeyboardMarkup HomeControllerIndexButtons(this ControllerContext context)
     {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {                
                new KeyboardButton("Login"),
            },
            new List<KeyboardButton>()
            {                
                new KeyboardButton("Ota-ona sifatida tashrif buyurish"),
            },
            new List<KeyboardButton>()
            {                
                new KeyboardButton("Bot haqida"),
            }
        };
        return MakeDefaultReplyKeyboardMarkup(buttons);
     }
    
    public static ReplyKeyboardMarkup MakeTeacherAttendanceSingleClassReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Darsga qatnashmaganlik xabari⛔️")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Darsga kechikib kirganligi  xabari⚠️"),
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Ortga")
            }
        };

        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    public static ReplyKeyboardMarkup ParentConfirmTheLogOutReplayKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Baribir chiqishni xohlayman")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Bosh menyuga qaytish"),
            }
        };

        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    

    private static ReplyKeyboardMarkup MakeDefaultReplyKeyboardMarkup(List<List<KeyboardButton>> buttons)
    {
        return new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
    }
    private static ReplyKeyboardMarkup MakeDefaultInlineKeyboardMarkup(List<List<KeyboardButton>> buttons)
    {
        return new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
    }

    public static ReplyKeyboardMarkup MakeDefaultReplyKeyboardMarkup(params KeyboardButton[] buttons)
    {
        return new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
    }

    public static ReplyKeyboardMarkup RequestPhoneNumberReplyKeyboardMarkup(this ControllerContext context)
        => MakeDefaultReplyKeyboardMarkup(new KeyboardButton("Telefon raqamimni yuborish") { RequestContact = true });
    
    public static ReplyKeyboardMarkup ParentControllerBackReplyKeyboardMarkup(this ControllerContext context)
        => MakeDefaultReplyKeyboardMarkup(new KeyboardButton("Ortga"));

    public static ReplyKeyboardMarkup ParentControllerBackToMainMenuReplyKeyboardMarkup(this ControllerContext context)
        => MakeDefaultReplyKeyboardMarkup(new KeyboardButton("Bosh menyuga qaytish"));

  //  public static ReplyKeyboardMarkup ParentMenuReplyKeyboardMarkup(this ControllerContext context)
    //    => MakeDefaultReplyKeyboardMarkup(new KeyboardButton(,new KeyboardButton(""),new KeyboardButton("Hisobdan chiqish "));

    
    public static ReplyKeyboardMarkup ParentMenuReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Boshqa farzandlar uchun ham ro'yxatdan o'tish")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Farzandlar ro'yxatini ko'rish"),
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Hisobdan chiqish "),
            }
        };

        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    
    
    public static InlineKeyboardMarkup MakeGetAllClassNumbersInlineReplyMarkup(this ControllerContext context, List<string> buttonLabels)
    {
        var sortedLabels = buttonLabels.OrderBy(label => int.Parse(label.Substring(0, label.IndexOf("-")))) // Raqamlar bo'yicha tartiblash
            .ThenBy(label => label.Substring(label.IndexOf("-") + 1)); // Harflar bo'yicha tartiblash

        var inlineKeyboard = new List<List<InlineKeyboardButton>>();

        foreach (var label in sortedLabels)
        {
            var button = InlineKeyboardButton.WithCallbackData(label);
            var row = new List<InlineKeyboardButton> { button };
            inlineKeyboard.Add(row);
        }

        return new InlineKeyboardMarkup(inlineKeyboard);
    }


   

   
    
    // public static InlineKeyboardMarkup MakeStudentsInlineKeyboardMarkup(this ControllerContext context,List<Student> students)
    // {
    //     IEnumerable<InlineKeyboardButton> buttons = students
    //         .Select(s =>
    //             new InlineKeyboardButton(s.FirstName+" "+s.LastName)
    //             {
    //                 CallbackData = s.Id.ToString(),
    //             });
    //
    //     return new InlineKeyboardMarkup(buttons);
    // }
    public static InlineKeyboardMarkup MakeStudentsInlineKeyboardMarkup(this ControllerContext context, List<Student> students)
    {
        var buttons = students.Select(s =>
                new InlineKeyboardButton(s.FirstName + " " + s.LastName)
                {
                    CallbackData = s.Id.ToString(),
                })
             .ToList();
       buttons.Add(new InlineKeyboardButton("Ortga")
       {
           CallbackData ="Ortga",
       });
        
        var inlineKeyboard = new List<List<InlineKeyboardButton>>();
        for (int i = 0; i < buttons.Count; i += 2)
        {
            var row = buttons.Skip(i).Take(2).ToList();
            inlineKeyboard.Add(row);
        }

        return new InlineKeyboardMarkup(inlineKeyboard);
    }
    
    public static ReplyKeyboardMarkup ConfirmationOfParentageReplayKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("Xa✅"),
                new KeyboardButton("Yo'q❌")
            }        
        };
        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    
    // public static InlineKeyboardMarkup FindBoardsReplyKeyboardMarkup(this ControllerContext context)
    // {
    //     var inlineButton = new InlineKeyboardButton("find")
    //     {
    //         SwitchInlineQueryCurrentChat = ""
    //     };
    //
    //     return new InlineKeyboardMarkup(inlineButton);
    // }

    public static ReplyKeyboardMarkup StudentsDepartmentReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("O'quvchi qo'shish"),
                new KeyboardButton("Excel orqali qo'shish")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("O'quvchi o'chirish"),
                new KeyboardButton("Sinfni o'chirish")
            },
            new List<KeyboardButton>()
            {
            new KeyboardButton("Ortga")
            }
            
        };
        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    public static ReplyKeyboardMarkup TeachersDepartmentReplyKeyboardMarkup(this ControllerContext context)
    {
        var buttons = new List<List<KeyboardButton>>()
        {
            new List<KeyboardButton>()
            {
                new KeyboardButton("O'qituvchi qo'shish"),
                new KeyboardButton("O'qituvchini o'chirish"),
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("O'qituvchilarni excel orqali qo'shish")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Barcha o'qituvchilar ro'yxatini olish")
            },
            new List<KeyboardButton>()
            {
                new KeyboardButton("Ortga")
            }
            
        };
        return MakeDefaultReplyKeyboardMarkup(buttons);
    }
    public static ReplyKeyboardMarkup Back(this ControllerContext context)
        => MakeDefaultReplyKeyboardMarkup(new KeyboardButton("Ortga"));

    public static ReplyKeyboardMarkup LogOut(this ControllerContext context)
        => MakeDefaultReplyKeyboardMarkup(new KeyboardButton("Ortga"));
    
       
}