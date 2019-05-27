using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBot.Models;
using TelegramBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Commands
{
    public class ReturnToChatCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public ReturnToChatCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/returntochat";

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} return to chat command");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            if (!user.InChat)
            {
                _logger.LogWarning("user not in chat chat");
                return;
            }

            user.State = _botContext.OwnerUser == user ? (int)State.InChatOwner : (int)State.InChat;
            client.SendTextMessageAsync(user.ChatId, "Вы вернулись в чат");

        }
    }
}
