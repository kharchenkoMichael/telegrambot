using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBot.Models;
using TelegramBot.Servecies;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBot.Commands
{
    public class SendMessageCommand : Command
    {
        private readonly BotContext _botContext;
        private readonly ILogger<Bot> _logger;

        public SendMessageCommand(BotContext botContext, ILogger<Bot> logger)
        {
            _botContext = botContext;
            _logger = logger;
        }

        public override string Name { get; } = "/sendmessage";

        public override bool Contains(string command)
        {
            return string.IsNullOrEmpty(command) || command[0] != '/';
        }

        public override void Execute(Message message, TelegramBotClient client)
        {
            if (string.IsNullOrEmpty(message.Text) 
                && message.Audio != null
                && message.Animation != null
                && message.Document != null
                && message.Contact != null
                && message.Location != null
                && message.Photo != null
                && message.NewChatPhoto != null
                && message.Sticker != null
                && message.Video != null)
                return;
            _logger.LogInformation($"{message.From.FirstName} send message");
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
            
            user.UpdateBalance(_logger);
            var messageCount = _botContext.GetMessageByBalance(user.Balance) - _botContext.Messages.Count(item => item.UserId == user.Id && item.Time.Date == DateTime.Today);

            if (messageCount == 0)
            {
                if (string.IsNullOrEmpty(user.BinanceApi))
                {
                    client.SendTextMessageAsync(user.ChatId, "У вас законьчилось количество сообщений на сегодня, добавьте ключи от биржи Binance, для просмотра баланса - /addbinance");
                    return;
                }
                var builder = new StringBuilder();
                builder.AppendLine("У вас законьчилось количество сообщений на сегодня, пополните балан, либо дождитесь завтрашнего дня");
                foreach (var s in _botContext.MessageInDayDictionary.Select(item => $"баланс > {item.Key}, тогда {item.Value} сообщений в день"))
                    builder.AppendLine(s);

                client.SendTextMessageAsync(user.ChatId, builder.ToString());
                return;
            }
            SendText(message, client, user);
            client.SendTextMessageAsync(user.ChatId, $"сообщение отправлено, осталось {messageCount - 1} сообщений");
        }

        private void SendText(Message message, TelegramBotClient client, User user)
        {
            var mes = new Models.Message
            {
                Id = message.MessageId,
                UserId = user.Id,
                Time = DateTime.Now,
                ContentText = message.Text
            };

            foreach (var chatUser in _botContext.Users.Where(item => item.InChat))
            {
                if (chatUser == user)
                    continue;

                client.SendTextMessageAsync(chatUser.ChatId, $"**{user.FirstName}** @{user.Name}: {message.Text}");

                if (message.Audio != null)
                {
                    client.SendAudioAsync(chatUser.ChatId, message.Audio.FileId);
                    mes.AudiId = message.Audio.FileId;
                }

                if (message.Animation != null)
                {
                    client.SendAnimationAsync(chatUser.ChatId, message.Animation.FileId);
                    mes.AnimationId = message.Animation.FileId;
                }

                if (message.Document != null)
                {
                    client.SendDocumentAsync(chatUser.ChatId, message.Document.FileId);
                    mes.DocumentId = message.Document.FileId;
                }

                if (message.Contact != null)
                    client.SendContactAsync(chatUser.ChatId, message.Contact.PhoneNumber, message.Contact.FirstName);

                if (message.Location != null)
                    client.SendLocationAsync(chatUser.ChatId, message.Location.Latitude, message.Location.Longitude);

                if (message.NewChatPhoto != null)
                {
                    mes.PhotoIds = new string[message.Photo.Length];
                    for (var i = 0; i < message.Photo.Length; i++)
                    {
                        var photoSize = message.Photo[i];
                        client.SendPhotoAsync(chatUser.ChatId, photoSize.FileId);
                        mes.PhotoIds[i] = photoSize.FileId;
                    }
                }

                if (message.Photo != null)
                {
                    mes.PhotoIds = new string[message.Photo.Length];
                    for (var i = 0; i < message.Photo.Length; i++)
                    {
                        var photoSize = message.Photo[i];
                        client.SendPhotoAsync(chatUser.ChatId, photoSize.FileId);
                        mes.PhotoIds[i] = photoSize.FileId;
                    }
                }

                if (message.Sticker != null)
                {
                    client.SendStickerAsync(chatUser.ChatId, message.Sticker.FileId);
                    mes.StickerId = message.Sticker.FileId;
                }

                if (message.Video != null)
                {
                    client.SendVideoAsync(chatUser.ChatId, message.Video.FileId);
                    mes.VideoId = message.Video.FileId;
                }

            }

            _botContext.Messages.Add(mes);
        }
    }
}
