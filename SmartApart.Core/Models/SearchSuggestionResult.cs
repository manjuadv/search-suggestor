using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApart.Core.Models
{
    public class SearchSuggestionResult
    {
        public long EntityID { get; set; }
        public string Suggestion { get; set; }
        public string FormerName { get; set; }
        public string SourceType { get; set; }
        public double? Rank { get; set; }
        public SearchEntity Source { get; set; }
    }
}
