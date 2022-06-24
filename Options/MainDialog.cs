using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class MainDialog
    {
        private static readonly List<string> spamList = new() { "http:", "https:", "wwww." };

        public static async Task Menu(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            bool spamChecker = spamList.Any(spam => message!.Text!.ToLower().Contains(spam));

            if (spamChecker)
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            var action = SendInlineKeyboard(botClient, message);

            Message sentMessage = await action;
        }
        private static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(Replies.AddToQueue, Replies.AddToQueue),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(Replies.AskPositions, Replies.AskPositions),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(Replies.DeleteFromQueue, Replies.DeleteFromQueue),
                        },
                });
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: Replies.Switch,
                                                        replyMarkup: inlineKeyboard);
        }
    }
}
