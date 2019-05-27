using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBot.Models;
using TelegramBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Commands
{
    public class SetBinanceKeyCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public SetBinanceKeyCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/setbinancekey";

        public override bool Contains(string command)
        {
            return !string.IsNullOrEmpty(command) && command[0] != '/';
        }

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} set binance key");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            var binance = message.Text.Split(':').ToList();
            if (binance.Count != 2)
            {
                client.SendTextMessageAsync(user.ChatId, "введите в правильном формате api:key, либо вернитесь обратно в чат с помощью команды /returntochat");
                return;
            }

            user.BinanceApi = binance[0];
            user.SecretKey = binance[1];
            user.UpdateBalance(_logger);

            user.State = _botContext.OwnerUser == user ? (int) State.InChatOwner : (int) State.InChat;
            var messageCount = _botContext.GetMessageByBalance(user.Balance) - _botContext.Messages.Count(item => item.UserId == user.Id && item.Time.Date == DateTime.Today);
            client.SendTextMessageAsync(user.ChatId, $"успешно ввели ключь ваш баланс {user.Balance}, сегодня вы можете отправить {messageCount} сообщений");
        }
    }
}
