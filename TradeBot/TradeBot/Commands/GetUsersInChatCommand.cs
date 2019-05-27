using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class GetUsersInChatCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public GetUsersInChatCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/getusersinchat";
        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} get users in chat ");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }
            if (!user.InChat)
            {
                _logger.LogWarning("user not in chat");
                return;
            }

            var builder = new StringBuilder();
            foreach (var userInChat in _botContext.Users.Where(item => item.InChat))
                builder.AppendLine($"**{userInChat.FirstName}** @{userInChat.Name}");

            client.SendTextMessageAsync(user.ChatId, builder.ToString());
            
        }
    }
}
