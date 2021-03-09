using System;
using System.Collections.Generic;
using System.Text;

namespace TerraFetcher.Models
{
    public class Setting
    {
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public List<string> LineTokens { get; set; }
    }
}
