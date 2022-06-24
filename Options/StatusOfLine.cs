using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotFunction.Api;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class StatusOfLine
    {
        public static async Task<Message> Get(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            Dictionary<string, string> userPositions = await QueueRequests.GetUserPositions(callbackQuery!.From!.Id!.ToString());
            
            if(userPositions!.Count == 0)
            {
                return await botClient.SendTextMessageAsync
                    (callbackQuery!.Message!.Chat.Id,
                    $"Нou have not added positions to the queue yet.");
            }

            string positionsInfo = "";
            foreach (var pos in userPositions)
            {
                positionsInfo += $"{pos.Value},\n";
            }
            return await botClient.SendTextMessageAsync
                   (callbackQuery!.Message!.Chat.Id,
                   $"{Replies.UserPositionsStatus}{positionsInfo.Remove(positionsInfo.Length-2)}.");

        }
    }
}
