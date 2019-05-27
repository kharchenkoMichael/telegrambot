using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace TradeBot.Models
{
    public enum State
    {
        Start,
        InChat,
        InChatOwner,
        InBinanceAdd,
        InMessageSettings
    }

    public class BotContext
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly string _filePath = "Logs/logs-0.txt";

        public BotContext(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public List<User> Users { get; set; } = new List<User>();
        public List<Message> Messages { get; set; } = new List<Message>();

        public List<User> RequestList { get; set; } = new List<User>();

        public User OwnerUser { get; set; }

        public Dictionary<int, int> MessageInDayDictionary = new Dictionary<int, int>();

        public void UpdateFromJson()
        {
            using (var reader = new StreamReader(Path.Combine(_appEnvironment.ContentRootPath, _filePath)))
            {
                BotContext m = JsonConvert.DeserializeObject<BotContext>(reader.ReadToEnd());
                Users = m.Users;
                Messages = m.Messages;
                RequestList = m.RequestList;
                OwnerUser = m.OwnerUser;
            }
        }

        public void WriteToJson()
        {
            using (var writer = new StreamWriter(Path.Combine(_appEnvironment.ContentRootPath, _filePath)))
            {
                writer.Write(JsonConvert.SerializeObject(this));
            }
        }

        public int GetMessageByBalance(int userBalance)
        {
            return MessageInDayDictionary.Where(item => item.Key <= userBalance).OrderByDescending(item => item.Key).Select(item => item.Value).FirstOrDefault();
        }
        
    }
    
}
