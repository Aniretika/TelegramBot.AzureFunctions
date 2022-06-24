using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Api;
using TelegramBotFunction.Entities;
using TelegramBotFunction.Replics;

namespace TelegramBotFunction.Options
{
    public static class AddOptions
    {
        static private ConcurrentDictionary<long, string> Answers = new();
        static private Position? position = new();

        public static void ClearMessageHistory()
        {
            Answers.Clear();
        }

        public static async Task SendAddPosition(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup =
            new(
                     new[]
                     {
                            new KeyboardButton[] { $"{callbackQuery!.Message!.Chat.FirstName} {callbackQuery.Message.Chat.LastName}" },
                     })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true,
            };

            await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id,
                                                       text: Replies.NameAsking,
                                                       replyMarkup: replyKeyboardMarkup);

            Answers.TryAdd(callbackQuery.From.Id, Replies.NameAsking);
        }
     
        public static async Task OnAddMessageReceived(ITelegramBotClient botClient, Message message)
        {
            long userId = message!.From!.Id;

            if (Answers.TryGetValue(userId, out var answer))
            {
                if (answer == Replies.NameAsking)
                {
                    position!.Requester = message.Text;
                    position.AuthorId = message.Chat.Id.ToString();
                    position.BotId = botClient.BotId.ToString();
                    await botClient.SendTextMessageAsync(message!.Chat!, Replies.DescriptionAsking, replyMarkup: new ReplyKeyboardRemove());
                    //Answers.
                    Answers.TryUpdate(userId, Replies.DescriptionAsking, answer);
                }
                else if (answer == Replies.DescriptionAsking)
                {
                    position!.Description = message.Text;
                    Answers.Clear();
                    var requestStatus = await QueueRequests.AddPosititon(position);

                    if (requestStatus == System.Net.HttpStatusCode.OK)
                    {
                        var lastNumberInQueue = await QueueRequests.GetLastPosition();

                        await botClient.SendTextMessageAsync(message!.Chat!,
                            $"You have been successfully added to the queue using the name {position.Requester}! " +
                            $"\nThe description of what you want to print: {position.Description}.\nYour number in line: {lastNumberInQueue}");

                        string positionStatus = await QueueRequests.GetUsersPosititonStatus(message.Chat.Id.ToString());
                        await botClient.SendTextMessageAsync(message!.Chat!, $"{Replies.UsersTurn}{positionStatus}.");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message!.Chat!, $"Response error.");
                        await botClient.SendTextMessageAsync(message!.Chat!, Replies.DescriptionAsking);
                    }

                    await MainDialog.Menu(botClient, message);
                }
            }
            else
            {
                await Greetings.BotMessageReceived(botClient, message);
                await MainDialog.Menu(botClient, message);
            }
        }
    }
}
