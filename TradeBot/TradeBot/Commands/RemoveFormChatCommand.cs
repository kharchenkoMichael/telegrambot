using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TradeBot.Models;
using TradeBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TradeBot.Commands
{
    public class RemoveFromChatCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public RemoveFromChatCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/removefromchat";

        public override void Execute(Message message, TelegramBotClient client)
        {
            _logger.LogInformation($"{message.From.FirstName} remove from chat");
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

            if (_botContext.OwnerUser == user)
            {
                if (message.Text == Name)
                {
                    client.SendTextMessageAsync(message.Chat.Id, "Вы не можете выйти из чата, вы его владелец, передайте права владельца сначала - /makeowner @user_name");
                    return;
                }

                var removeUsers = message.Text.Split('@').Where(item => item != "/removefromchat");
                var removedUsers = new List<User>();
                foreach (var removeUser in _botContext.Users.Where(item => item.InChat && removeUsers.Contains(item.Name)))
                {
                    removedUsers.Add(removeUser);
                    removeUser.InChat = false;
                    removeUser.State = (int)State.Start;
                    client.SendTextMessageAsync(removeUser.ChatId, "Вас выгнали из чата");
                }

                foreach (var removeUser in removedUsers)
                {
                    foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
                        client.SendTextMessageAsync(chatUser.ChatId, $"**{removeUser.FirstName}** @{removeUser.Name} выгнали из чата");
                }
                return;
            }


            user.InChat = false;
            user.State = (int) State.Start;
            client.SendTextMessageAsync(message.Chat.Id, "Вы вышли из чата");

            foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
                client.SendTextMessageAsync(chatUser.ChatId, $"**{user.FirstName}** @{user.Name} вышел из чата");
        }
    }
}
