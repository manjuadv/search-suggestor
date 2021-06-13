using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApart.Core.Models
{
    public class SearchSuggestionResult
    {
        public string Suggestion { get; set; }
        public string SourceType { get; set; }
        public int Rank { get; set; }
    }
}
