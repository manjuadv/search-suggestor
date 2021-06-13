using ElasticsearchConnector.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using SmartApart.Core.Models;
using SmartApart.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly ILogger<PropertiesController> _logger;
        private readonly IElasticClient elasticClient;
        private readonly IConfiguration configuration;
        private readonly ISearchService searchService;

        public PropertiesController(ILogger<PropertiesController> logger, IElasticClient elasticClient, 
            IConfiguration configuration, ISearchService searchService)
        {
            _logger = logger;
            this.elasticClient = elasticClient;
            this.configuration = configuration;
            this.searchService = searchService;
        }
        /// <summary>
        /// Search within several markets (optional). eg : /properties?text=keyword&market=Market1&market=Market2
        /// </summary>
        /// <param name="text"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SearchSuggestionResult> Get(string text, [FromQuery] string[] market, int limit = 25)
        {
            if (market != null && market.Length < 1)
                market = null;
            if (limit < 1)
                limit = 25;
            return GetSearchResults(text, market, limit);
        }

        /// <summary>
        /// Search within specific Market (optional). eg : /properties/market/Atlanta?text=keyword
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("markets/{market}")]
        public IEnumerable<SearchSuggestionResult> Get(string text, string market="", int limit=25)
        {
            string[] markests = new string[] { market };
            if (string.IsNullOrEmpty(market))
                markests = null;
            if (limit < 1)
                limit = 25;
            return GetSearchResults(text, markests, limit);
        }
        private IEnumerable<SearchSuggestionResult> GetSearchResults(string text, string[] markets, int limit)
        {
            IEnumerable<SearchSuggestionResult> results = searchService.GetAutocompleteSuggestions(text, markets, size: limit, misspellingMaxAllowed: 1);
            return results;
        }
    }
}
