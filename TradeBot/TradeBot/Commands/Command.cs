using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract void Execute(Message message, TelegramBotClient client);

        public virtual bool Contains(string command)
        {
            return !string.IsNullOrEmpty(command) && command.Contains(Name);
        }

    }
}
