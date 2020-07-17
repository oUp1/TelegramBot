using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services;

namespace TelegramBot
{
    class Program
    {
        private static TelegramBotClient _client;
        private static CommandService _commandService;
        private static string _token = "971797779:AAEyzrWz4xIl942EoPzf2Mi94K0xLpEL8sU";
        static void Main(string[] args)
        {
            _client = new TelegramBotClient(_token);
            _client.OnMessage += BotOnMessageReceived;
            _client.OnMessageEdited += BotOnMessageReceived;
            _client.StartReceiving();

            Console.ReadLine();

            _client.StopReceiving();

        }
        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            _commandService = new CommandService();
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                var result = _commandService.ExecuteCommandAsync(message.Text);
                foreach (var msg in result)
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, $"{msg.ToString()}");
                }
            }
        }
    }
}

