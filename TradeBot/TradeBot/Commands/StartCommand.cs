using Telegram.Bot;
using TelegramBot.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Commands
{
    public class StartCommand : Command
    {
        private readonly BotContext _botContext;

        public StartCommand(BotContext botContext)
        {
            _botContext = botContext;
        }

        public override string Name { get; } = "/start";
        public override void Execute(Message message, TelegramBotClient client)
        {
            _botContext.Users.Add(new User
            {
                ChatId = message.Chat.Id,
                Name = message.From.Username,
                FirstName = message.From.FirstName,
                Id = message.From.Id,
                InChat = false,
                State = (int)State.Start
            });

            client.SendTextMessageAsync(message.Chat.Id,
                _botContext.OwnerUser == null
                    ? $"Привет {message.From.FirstName}, ты можешь вступить в чат и быть его создателем, нажми команду /addtochat"
                    : $"Привет {message.From.FirstName}, чтобы вступить в чат нажми команду '/addtochat' и жди покасоздатель чата примет твой запрос");
        }
    }
}
