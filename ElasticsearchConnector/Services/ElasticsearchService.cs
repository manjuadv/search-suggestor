using ElasticsearchConnector.Connectors;
using ElasticsearchConnector.Connectors.ManagementCompany;
using ElasticsearchConnector.Connectors.Property;
using Nest;
using SmartApart.Core.Models;
using SmartApart.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Services
{
    public class ElasticsearchService : ISearchService
    {
        private readonly IElasticClient elasticClient;
        private string indexName;

        public ElasticsearchService(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public IEnumerable<SearchSuggestionResult> GetAutocompleteSuggestions(string text, string[] markets = null)
        {

            IMgmtCompSearchConnector srchCon = new MgmtCompSearchConnector(elasticClient);
            IEnumerable<MgmtCompany> mgmtCompaiesResults = srchCon.AutoCompleteSearchByCustomAnalyzer(indexName, "Avan", market: null, size: 100);

            IPropertySearchConnector searchConnector = new PropertySearchConnector(elasticClient);
            IEnumerable<PropertyItem> propertiesResults = searchConnector.AutoCompleteSearchByCustomAnalyzer(indexName, text, size: 100, market: markets, misspellingMaxAllowed: 0);

            List<SearchSuggestionResult> resultsList = new List<SearchSuggestionResult>();

            foreach (var result in propertiesResults)
            {
                resultsList.Add(new SearchSuggestionResult { Suggestion = result.Name, SourceType = "property", Rank = 0});
            }
            return null;
        }
    }
}
