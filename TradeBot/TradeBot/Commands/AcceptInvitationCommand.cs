using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class AcceptInvitationCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public AcceptInvitationCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/accept";
        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} accept invitation ");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            if (_botContext.OwnerUser != user)
            {
                _logger.LogWarning("user is not owner");
                return;
            }

            var userNames = message.Text.Split('@').Where(item => item != "/accept");

            foreach (var userName in userNames)
            {
                var acceptUser = _botContext.Users.FirstOrDefault(item => item.Name == userName);
                if (acceptUser == null)
                {
                    _logger.LogWarning($"{userName} - can't find this user name");
                    continue;
                }

                if (!_botContext.RequestList.Contains(acceptUser))
                {
                    _logger.LogWarning($"{userName} - don't sent request");
                    continue;
                }

                _botContext.RequestList.Remove(acceptUser);

                foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
                    client.SendTextMessageAsync(chatUser.ChatId, $"**{acceptUser.FirstName}** @{acceptUser.Name} вступил в чат");

                acceptUser.InChat = true;
                acceptUser.State = (int)State.InChat;
                client.SendTextMessageAsync(acceptUser.ChatId, "Вы вступили в чат");
            }
        }
    }
}
