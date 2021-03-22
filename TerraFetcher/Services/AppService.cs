using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TerraFetcher.Models;

namespace TerraFetcher.Services
{
    public class AppService
    {
        private readonly MirrorService mirrorService;
        private readonly DatabaseService databaseService;
        private readonly LogService logService;

        public AppService()
        {
            mirrorService = new MirrorService();
            databaseService = new DatabaseService();
            logService = new LogService();
        }

        public void Run()
        {
            var now = DateTime.Now;

            Setting setting = GetSetting();
            var prices = mirrorService.GetCurrentPrice().Data.SelectMany(x => x.Assets);
            List<LogModel> logs = logService.GetLogs();

            StringBuilder message = ComposeNotifyMessage(setting, prices, logs);
            var sendingMessage = message.ToString();
            SendLineNotify(setting, sendingMessage);

            WriteDatabase(now, prices);

            WriteLog(prices);
        }

        private static Setting GetSetting()
        {
            var settings = File.ReadAllText("setting.json");
            var setting = JsonConvert.DeserializeObject<Setting>(settings);
            return setting;
        }

        private static StringBuilder ComposeNotifyMessage(Setting setting, IEnumerable<Asset> prices, List<LogModel> logs)
        {
            var favorites = prices.Where(x => setting.SymbolFavorites.Contains(x.Symbol))
                            .OrderBy(x => x.Prices.Spread)
                            .ToList();
            var filtereds = prices.Where(x => setting.SymbolFilters.Contains(x.Symbol) == false)
                .OrderBy(x => x.Prices.Spread)
                .ToList();
            var buys = filtereds.Where(x => x.Prices.Spread <= setting.Low && x.Prices.Spread >= 0.00M).ToList();
            var sells = filtereds.Where(x => x.Prices.Spread >= setting.High).ToList();

            var message = new StringBuilder("\n");
            if (favorites.Any())
            {
                ComposeMessage("Mirror Favorites:", favorites, logs, message);
            }

            if (buys.Any())
            {
                ComposeMessage("Mirror Buy:", buys, logs, message);
            }

            if (sells.Any())
            {
                ComposeMessage("Mirror Sell:", sells, logs, message);
            }

            return message;
        }

        private static void ComposeMessage(string header, List<Asset> assets, List<LogModel> logs, StringBuilder message)
        {
            message.AppendLine(header);
            foreach (var asset in assets)
            {
                var change = "";
                var log = logs.FirstOrDefault(x => x.Symbol == asset.Symbol);
                if (log != null)
                {
                    if (asset.Prices.OraclePriceAt > log.OraclePrice)
                    {
                        change = " (+)";
                    }
                    if (asset.Prices.OraclePriceAt < log.OraclePrice)
                    {
                        change = " (-)";
                    }
                }

                message.AppendLine($"- {asset.Symbol} {asset.Prices.Spread * 100:F}%: {asset.Prices.PriceAt:F}{change}");
            }
            message.AppendLine();
        }

        private static void SendLineNotify(Setting setting, string sendingMessage)
        {
            if (string.IsNullOrWhiteSpace(sendingMessage) == false)
            {
                var line = new LineNotify();
                foreach (var token in setting.LineTokens)
                {
                    line.Send(token, sendingMessage);
                }
            }
        }

        private void WriteDatabase(DateTime now, IEnumerable<Asset> prices)
        {
            var tickers = prices.Select(x => new TickerModel
            {
                Symbol = x.Symbol,
                DateTime = now,
                Price = x.Prices.PriceAt,
                OraclePrice = x.Prices.OraclePriceAt
            }).ToList();
            databaseService.Save(tickers);
        }

        private void WriteLog(IEnumerable<Asset> prices)
        {
            var newLogs = prices.Select(x => new LogModel
            {
                Symbol = x.Symbol,
                OraclePrice = x.Prices.OraclePriceAt
            }).ToList();
            logService.WriteLogs(newLogs);
        }
    }
}