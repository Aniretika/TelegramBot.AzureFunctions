using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class Greetings
    {
        public static async Task<Message> BotMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(message!.Chat!, Replies.Greeting);
        }

    }
}
