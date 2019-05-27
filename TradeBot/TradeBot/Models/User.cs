using System.Linq;
using Binance.API.Csharp.Client;
using Microsoft.Extensions.Logging;

namespace TradeBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public long ChatId { get; set; }
        public int State { get; set; }
        public bool InChat { get; set; }
        public string BinanceApi { get; set; }
        public string SecretKey { get; set; }
        public int Balance { get; set; }

        public void Update(Telegram.Bot.Types.Message message)
        {
            Name = message.From.Username;
            FirstName = message.From.FirstName;
            ChatId = message.Chat.Id;
        }

        public void UpdateBalance(ILogger logger)
        {
            Balance = 0;
            if (string.IsNullOrEmpty(BinanceApi))
                return;
            var apiClient = new ApiClient(BinanceApi, SecretKey);
            var binanceClient = new BinanceClient(apiClient);

            var accountInfo = binanceClient.GetAccountInfo();
            var balances = accountInfo.Result.Balances;

            var order = binanceClient.GetOrderBookTicker().Result.ToList();
            
            foreach (var balance in balances)
            {
                if (balance == null)
                    continue;
                if (balance.Asset.Contains("usd"))
                {
                    Balance += (int)(balance.Free + balance.Locked);
                    continue;
                }

                var btc = 0m;
                var i = order.FirstOrDefault(item => item.Symbol.ToUpper().Contains((balance.Asset + "btc").ToUpper()));
                if (i != null)
                    btc += i.AskPrice * (balance.Free + balance.Locked);
                else
                    logger.LogInformation((balance.Asset + "btc").ToUpper());

                Balance += (int)(btc * order.First(item => item.Symbol.ToUpper() == "BTCUSDT").AskPrice);
            }
        }
    }
}
