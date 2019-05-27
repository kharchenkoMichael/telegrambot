using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class HelpCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public HelpCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/help";

        public override bool Contains(string command)
        {
            return true;
        }

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} help");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            client.SendTextMessageAsync(message.Chat.Id, GetHelpText((State)user.State));
        }

        private string GetHelpText(State state)
        {
            switch (state)
            {
                case State.Start:
                    return "Чтобы вступить в чат нажми /addtochat";
                case State.InChat:
                    return "Вы в чате, пишите сообщения и их увидят все члены чата, чтобы выйт" +
                           "и из чата нажмите /removefromchat, посмотреть кто есть в чате /getu" +
                           "sersinchat, добавить ключи от биржи /addbinance";
                case State.InChatOwner:
                    return "Вы глава чата, вы можете принимать заявки в чат - /accept_@userNam" +
                           "e и отклонять - /reject_userName, сделать главой другого учасника " +
                           "клана - /makeowner_@userName, удалить других из чата /removeformcha" +
                           "t_@userNameпосмотреть кто есть в чате /getusersinchat, добавить клю" +
                           "чи от биржи /addbinance, добавить настройки сообщений /addmessagesettings";
                case State.InBinanceAdd:
                    return "Вы в меню добавления ключей от биржи. Их нужно добавить в таком " +
                           "формате - api:key, чтобы вернуться в чат нажмите /returntochat";
                case State.InMessageSettings:
                    return "Вы в меню добавления настроек сообщений в" +
                           "формате - \n0:2\n10-10\n, чтобы вернуться в чат нажмите /returntochat";
                default:
                    _logger.LogWarning($"default state {(int)state}");
                    return string.Empty;
            }
        }
    }
}
