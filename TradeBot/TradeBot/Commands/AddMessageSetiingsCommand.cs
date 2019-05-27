using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class AddMessageSettingsCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public AddMessageSettingsCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/addmessagesettings";
        
        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} add binance key ");
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

            user.State = (int)State.InMessageSettings;
            client.SendTextMessageAsync(user.ChatId, "Введите настройки в таком формате\n0-2\n10-10");
        }
    }
}
