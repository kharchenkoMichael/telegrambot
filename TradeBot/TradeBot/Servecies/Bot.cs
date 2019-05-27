using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Commands;
using TradeBot.Models;

namespace TradeBot.Servecies
{
    public class Bot
    {
        private readonly AppSettings _settings;
        private readonly ILogger<Bot> _logger;
        private readonly BotContext _botContext;
        private TelegramBotClient client;

        public Dictionary< /**/int, List<Command>> GetCommands()
        {
            var commands = new Dictionary<int, List<Command>>();
            commands[(int)State.Start] = new List<Command>
            {
                new AddToChatCommand(_botContext, _logger),
                new HelpCommand(_botContext, _logger)
            };

            commands[(int)State.InChat] = new List<Command>
            {
                new RemoveFromChatCommand(_botContext, _logger),
                new SendMessageCommand(_botContext, _logger),
                new AddBinanceKeyCommand(_botContext, _logger),
                new GetUsersInChatCommand(_botContext, _logger),
                new HelpCommand(_botContext, _logger)
            };

            commands[(int)State.InChatOwner] = new List<Command>
            {
                new RemoveFromChatCommand(_botContext, _logger),
                new SendMessageCommand(_botContext, _logger),
                new AcceptInvitationCommand(_botContext, _logger),
                new RejectInvitationCommand(_botContext, _logger),
                new MakeOwnerCommand(_botContext, _logger),
                new RemoveFromChatCommand(_botContext, _logger),
                new GetUsersInChatCommand(_botContext, _logger),
                new AddBinanceKeyCommand(_botContext, _logger),
                new AddMessageSettingsCommand(_botContext, _logger),
                new HelpCommand(_botContext, _logger)
            };
            commands[(int)State.InBinanceAdd] = new List<Command>
            {
                new SetBinanceKeyCommand(_botContext, _logger),
                new ReturnToChatCommand(_botContext, _logger),
                new HelpCommand(_botContext, _logger)
            };
            commands[(int)State.InMessageSettings] = new List<Command>
            {
                new SetMessageSettingsCommand(_botContext, _logger),
                new ReturnToChatCommand(_botContext, _logger),
                new HelpCommand(_botContext, _logger)
            };
            return commands;
        }

        public Bot(AppSettings settings, ILogger<Bot> logger, BotContext botContext)
        {
            _settings = settings;
            _logger = logger;
            _botContext = botContext;
        }

        public async Task<TelegramBotClient> Get()
        {
            if (client != null)
            {
                return client;
            }
            client = new TelegramBotClient(_settings.Key);
            _logger.LogInformation("Get Bot");
            var hook = string.Format(_settings.Url, "api/message/update");
            await client.SetWebhookAsync(hook);

            return client;
        }
    }
}
