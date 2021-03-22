using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using TerraFetcher.Models;

namespace TerraFetcher.Services
{
    public class DatabaseService
    {
        private string filePath = Path.Combine(Directory.GetCurrentDirectory(), "mirror.db");

        public void Save(List<TickerModel> tickers)
        {
            using (var db = new LiteDatabase(filePath))
            {
                var col = db.GetCollection<TickerModel>("tickers");

                col.InsertBulk(tickers);

                col.EnsureIndex(x => x.DateTime);
            }
        }

        public List<TickerModel> Get(string symbol, DateTime from)
        {
            using (var db = new LiteDatabase(filePath))
            {
                var col = db.GetCollection<TickerModel>("tickers");

                var results = col.Query()
                    .Where(x => x.Symbol == symbol)
                    .Where(x => x.DateTime >= from)
                    .OrderBy(x => x.DateTime)
                    .ToList();

                return results;
            }
        }
    }
}