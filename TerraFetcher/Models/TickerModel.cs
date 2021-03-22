using System;

namespace TerraFetcher.Models
{
    public class TickerModel
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
        public decimal OraclePrice { get; set; }
    }
}