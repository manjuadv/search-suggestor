using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApart.Core.Services
{
    public interface ISearchService
    {
        IEnumerable<SearchSuggestionResult> GetAutocompleteSuggestions(string text, string[] markets = null, int size = 25, 
            int misspellingMaxAllowed = 2);
    }
}
