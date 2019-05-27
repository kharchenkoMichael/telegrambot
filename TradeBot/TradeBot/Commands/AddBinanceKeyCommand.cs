using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class AddBinanceKeyCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public AddBinanceKeyCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/addbinance";

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} add binance key ");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            user.State = (int) State.InBinanceAdd;
            client.SendTextMessageAsync(user.ChatId, "Введите binance api b secret api в таком формате api:key");
        }
    }
}
