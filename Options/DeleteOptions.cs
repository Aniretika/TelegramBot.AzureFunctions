using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Api;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class DeleteOptions
    {
        public static async Task OnDeleteMessageReceived(ITelegramBotClient botClient, Message message) => await MainDialog.Menu(botClient, message);
        
        public static async Task SendDeletePosition(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            Dictionary<string, string> userPositions = await QueueRequests.GetUserPositions(authorId: callbackQuery!.From!.Id!.ToString());
            if (userPositions!.Count == 0)
            {
                Message noItemMessage = await botClient.SendTextMessageAsync(callbackQuery!.Message!.Chat.Id, "No items to delete.");
                await OnDeleteMessageReceived(botClient, noItemMessage);
                return;
            }

            var inlineKeyboardMarkup = InlineKeyboardMarkupMaker(userPositions);
            await botClient.SendTextMessageAsync(chatId: callbackQuery!.Message!.Chat.Id, text: Replies.SelectForDeleting, replyMarkup: inlineKeyboardMarkup);
        }

        public static InlineKeyboardMarkup InlineKeyboardMarkupMaker(Dictionary<string, string> items)
        {
            InlineKeyboardButton[][] inlineKeyboard = items.Select(item => new[]
            {
                new InlineKeyboardButton(item.Value)
                {
                    CallbackData = item.Key,
                }
            } ).ToArray();
            return new InlineKeyboardMarkup(inlineKeyboard);
        }
    }
}
