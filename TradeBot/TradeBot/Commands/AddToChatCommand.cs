using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class AddToChatCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public AddToChatCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/addtochat";
        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} add to chat");
            var user = _botContext.Users.FirstOrDefault(item => item.Id == message.From.Id);
            if (user == null)
            {
                _logger.LogWarning("user is null");
                return;
            }

            if (user.InChat)
            {
                _logger.LogWarning("user already in chat");
                return;
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                client.SendTextMessageAsync(message.Chat.Id, "Создайте userName");
                return;
            }


            if (_botContext.RequestList.Contains(user))
                client.SendTextMessageAsync(message.Chat.Id, "Вы уже подали запрос на вступление");
            else
            {
                if (_botContext.OwnerUser == null)
                {
                    foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
                        client.SendTextMessageAsync(chatUser.ChatId, $"**{user.FirstName}** @{user.Name} вступил в чат");

                    user.InChat = true;
                    user.State = (int) State.InChatOwner;
                    _botContext.OwnerUser = user;
                    client.SendTextMessageAsync(message.Chat.Id, "Вы вступили в чат");
                }
                else
                {
                    _botContext.RequestList.Add(user);
                    client.SendTextMessageAsync(message.Chat.Id, "Заявка на вступление успешно подана");
                    client.SendTextMessageAsync(_botContext.OwnerUser.ChatId, $"**{user.FirstName}** хочет вступить в чат, чтобы принфть нажмите /accept@{user.Name}, чтобы удалить нажмите /reject@{user.Name}");
                }
            }
        }
    }
}
