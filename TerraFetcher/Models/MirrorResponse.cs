using System.Collections.Generic;

namespace TerraFetcher.Models
{
    public class MirrorResponse
    {
        public List<Data> Data { get; set; }
    }

    public class Data
    {
        public List<Asset> Assets { get; set; }
    }

    public class Asset
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public Price Prices { get; set; }
    }

    public class Price
    {
        public decimal PriceAt { get; set; }
        public decimal OraclePriceAt { get; set; }
        public decimal Spread => PriceAt > 0 && OraclePriceAt > 0 ? (PriceAt / OraclePriceAt) - 1 : 0;
    }
}