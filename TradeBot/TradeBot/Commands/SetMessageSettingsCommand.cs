using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBot.Models;
using TelegramBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Commands
{
    public class SetMessageSettingsCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public SetMessageSettingsCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/setmessageSettings";

        public override bool Contains(string command)
        {
            return !string.IsNullOrEmpty(command) && command[0] != '/';
        }

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} set message settings");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            _botContext.MessageInDayDictionary = new Dictionary<int, int>();
            var settings = message.Text.Split('\n').ToList();
            foreach (var setting in settings)
            {
                var s = setting.Split('-');
                if (s.Length != 2)
                    continue;
                _botContext.MessageInDayDictionary[int.Parse(s[0])] = int.Parse(s[1]);
            }

            user.State = (int) State.InChatOwner;
            client.SendTextMessageAsync(user.ChatId, "настройки установлены");
            foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
            {
                if (chatUser == user)
                    continue;

                var builder = new StringBuilder();
                builder.AppendLine("Изменены настройки:");
                foreach (var s in _botContext.MessageInDayDictionary.Select(item =>
                    $"Баланс > {item.Key}, тогда {item.Value} сообщений в день."))
                    builder.AppendLine(s);
            }
        }
    }
}
