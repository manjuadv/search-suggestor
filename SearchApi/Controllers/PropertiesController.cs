using ElasticsearchConnector.Services;
using Microsoft.AspNetCore.Mvc;
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

        public PropertiesController(ILogger<PropertiesController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Search within several markets (optional). eg : /properties?text=keyword&market=Market1&market=Market2
        /// </summary>
        /// <param name="text"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get(string text, [FromQuery] string[] market, int limit= 25)
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Search within specific Market (optional). eg : /properties/market/Atlanta?text=keyword
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("markets/{market}")]
        public string Get(string text, string market="", int limit=25)
        {
            string[] markests = new string[] { market };
            if (string.IsNullOrEmpty(market))
                markests = null;

            string indexProperty = "property5";
            string indexMgmt = "mgmt4";
            Uri url = new Uri("http://localhost:9200/");
            ConnectionSettings settings = new ConnectionSettings(url);
            settings.EnableDebugMode();
            IElasticClient elasticClient = new ElasticClient(settings);
            ISearchService searchService = new ElasticsearchService(elasticClient, indexProperty, indexMgmt);
            IEnumerable<SearchSuggestionResult> results = searchService.GetAutocompleteSuggestions(text, markests);
            return "value";
        }
    }
}
