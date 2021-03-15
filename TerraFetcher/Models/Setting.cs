using System.Collections.Generic;

namespace TerraFetcher.Models
{
    public class Setting
    {
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public List<string> SymbolFavorites { get; set; }
        public List<string> SymbolFilters { get; set; }
        public List<string> LineTokens { get; set; }
    }
}