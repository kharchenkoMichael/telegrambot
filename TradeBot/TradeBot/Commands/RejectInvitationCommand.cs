using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class RejectInvitationCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public RejectInvitationCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/reject";
        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} reject invitation ");
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

            var userNames = message.Text.Split('@').Where(item => item != "/reject");

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
                
                client.SendTextMessageAsync(acceptUser.ChatId, "Вас не приняли в чат");
            }
        }
    }
}
