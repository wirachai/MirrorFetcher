using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using TerraFetcher.Models;
using TerraFetcher.Services;

namespace TerraFetcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var settings = File.ReadAllText("setting.json");
            var setting = JsonConvert.DeserializeObject<Setting>(settings);

            var service = new MirrorService();
            var prices = service.GetCurrentPrice().Data.SelectMany(x => x.Assets)
                .Where(x => setting.SymbolFilters.Contains(x.Symbol) == false)
                .OrderBy(x => x.Prices.Spread)
                .ToList();

            var buys = prices.Where(x => x.Prices.Spread <= setting.Low && x.Prices.Spread >= 0.00M).ToList();
            var sells = prices.Where(x => x.Prices.Spread >= setting.High).ToList();

            var message = new StringBuilder("\n");
            if (buys.Any())
            {
                message.AppendLine("Mirror Buy:");
                foreach (var asset in buys)
                {
                    message.AppendLine($"- {asset.Symbol} {asset.Prices.Spread * 100:F}%: {asset.Prices.PriceAt:F}");
                }
                message.AppendLine();
            }

            if (sells.Any())
            {
                message.AppendLine("Mirror Sell:");
                foreach (var asset in sells)
                {
                    message.AppendLine($"- {asset.Symbol} {asset.Prices.Spread * 100:F}%: {asset.Prices.PriceAt:F}");
                }
                message.AppendLine();
            }

            var sendingMessage = message.ToString();
            if (string.IsNullOrWhiteSpace(sendingMessage) == false)
            {
                var line = new LineNotify();
                foreach (var token in setting.LineTokens)
                {
                    line.Send(token, sendingMessage);
                }
            }
        }
    }
}