using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBot.Models;
using TelegramBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Commands
{
    public class MakeOwnerCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public MakeOwnerCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/makeowner";

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} make owner ");
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

            var userName = message.Text.Split('_').First(item => item[0] == '@').Substring(1);

            var ownerUser = _botContext.Users.FirstOrDefault(item => item.Name == userName);
            if (ownerUser == null)
            {
                _logger.LogWarning($"{userName} - can't find this user name");
                return;
            }

            if (!ownerUser.InChat)
            {
                _logger.LogWarning($"{userName} - not in chat");
                return;
            }
            
            foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
                client.SendTextMessageAsync(chatUser.ChatId,
                    $"**{ownerUser.FirstName}** @{ownerUser.Name} стал владельцем чата");
            
            ownerUser.State = (int) State.InChatOwner;
            client.SendTextMessageAsync(ownerUser.ChatId, "Вы стали владельцем чата");
            _botContext.OwnerUser = ownerUser;
        }
    }
}
