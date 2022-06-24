using Microsoft.Extensions.Logging;
using System.Net;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFunction.Api;
using TelegramBotFunction.Options;
using TelegramBotFunction.Replics;

namespace Telegram.Bot.Examples.AzureFunctions.WebHook
{

    public class UpdateService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(ITelegramBotClient botClient, ILogger<UpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }

        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(_botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(_botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(_botClient, update.CallbackQuery!),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {

            if (message.Type == MessageType.Text)
            {
                await AddOptions.OnAddMessageReceived(botClient, message);
            }
        }

        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"You have chosen {callbackQuery.Data}");

            await BotOnCallbackOptionalReceived(botClient, callbackQuery);
        }


        private static async Task BotOnCallbackOptionalReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //await botClient.EditMessageReplyMarkupAsync(callbackQuery.InlineMessageId);
            switch (callbackQuery.Data)
            {

                case Replies.AddToQueue:
                    {
                        AddOptions.ClearMessageHistory();
                        await AddOptions.SendAddPosition(botClient, callbackQuery);
                        break;
                    }

                case Replies.AskPositions:
                    {
                        AddOptions.ClearMessageHistory();
                        await StatusOfLine.Get(botClient, callbackQuery);
                        break;
                    }

                case Replies.DeleteFromQueue:
                    {
                        AddOptions.ClearMessageHistory();
                        await DeleteOptions.SendDeletePosition(botClient, callbackQuery);
                        break;
                    }
            }

            if (callbackQuery!.Message!.Text == Replies.SelectForDeleting)
            {
                HttpStatusCode deleteRequest = await QueueRequests.DeletePosition(callbackQuery!.Data!);
                if (deleteRequest == HttpStatusCode.OK)
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "The position was successfully deleted!");
                    await StatusOfLine.Get(botClient, callbackQuery);
                }
                else
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Something went wrong on the server. Try again later.");
                }
            }
        }

        private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation("Unknown update type: {updateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}
