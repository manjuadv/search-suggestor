using Elasticsearch.Net;
using ElasticsearchConnector.Connectors.Aws;
using Nest;
using Newtonsoft.Json;
using SmartApart.Core.Models;
using SmartApart.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data_Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = "properties.json";
            string filePath = @"..\..\..\..\DataFiles\";

            List<string> objectStrings = JsonReadHelper.GetObjectString(filePath + sourceFileName);
            List<PropertyItem> propertyList = JsonReadHelper.GetPropertyModelByJsonStrings(objectStrings, sourceFileName, allowDuplicateUpload: true);


            //Uri url = new Uri("https://propuser:data$1User@search-properties-fy3tslyamx44nmky4ezpycpaea.ap-south-1.es.amazonaws.com");
            Uri url = new Uri("http://localhost:9200/");
            ConnectionSettings settings = new ConnectionSettings(url);
            settings.EnableDebugMode();
            IElasticClient elasticClient = new ElasticClient(settings);
            
           
            string indexName = "property12";

            //CreateIndexAutoMap(elasticClient, indexName);

            //CreateIndexSearchSuggestion(elasticClient, indexName);

            //IndexPropertyItem(elasticClient, indexName, propertyList[0]);
            //SearchMatchPrase(elasticClient, indexName, "Abilene");
            //SearchTerm(elasticClient, indexName, "Abilene");
            //SearchMatchPrase(elasticClient, indexName, "Cur"); // No match


            //SearchMatchPrefixPhase(elasticClient, indexName, "Ranc");

            //IndexPropertyItemBulkAll(elasticClient, indexName, propertyList, 10);

            //SearchMatchPrefixPhase(elasticClient, indexName, "Stone R", 100, null);//"Atlanta");
            SearchMatchPrefixPhase(elasticClient, indexName, "Mead", 20, null);
        }
        private static void CreateIndexAutoMap(IElasticClient elasticClient, string indexName)
        {
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.CreateIndexAutoMap(indexName);
        }
        private static void CreateIndexSearchSuggestion(IElasticClient elasticClient, string indexName)
        {
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.CreateSearchSuggetionIndex(indexName);
        }
        private static void IndexPropertyItem(IElasticClient elasticClient, string indexName, PropertyItem proerpty)
        {
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.IndexRecord(proerpty, indexName);
        }
        private static void IndexPropertyItemBulkAll(IElasticClient elasticClient, string indexName, List<PropertyItem> propertis, int itemsPerRequest)
        {          
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.IndexRecordsBulkAll(propertis, indexName, itemsPerRequest);
        }
        private static void IndexPropertyItemBulk(IElasticClient elasticClient, string indexName, List<PropertyItem> propertis)
        {           
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.IndexRecordsBulk(propertis, indexName);
        }

        private static void SearchMatchPrase(IElasticClient elasticClient, string indexName, string key)
        {           
            AwsElasticsearchSearchConnector searchConnector = new AwsElasticsearchSearchConnector(elasticClient);
            IEnumerable<PropertyItem> results = searchConnector.SearchByName(indexName, key);
            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
        private static void SearchTerm(IElasticClient elasticClient, string indexName, string key)
        {            
            AwsElasticsearchSearchConnector searchConnector = new AwsElasticsearchSearchConnector(elasticClient);
            IEnumerable<PropertyItem> results = searchConnector.FilterByName(indexName, key);

            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
        private static void SearchMatchPrefixPhase(IElasticClient elasticClient, string indexName, string key, int size, string market)
        {
            AwsElasticsearchSearchConnector searchConnector = new AwsElasticsearchSearchConnector(elasticClient);
            //IEnumerable<PropertyItem> results = searchConnector.AutoCompleteSearchSimple(indexName, key, size: size, market:market);

            //IEnumerable<PropertyItem> results = searchConnector.AutoCompleteNameByCustomAnalyzer(indexName, key, size: size, market: market);
            IEnumerable<PropertyItem> results = searchConnector.AutoCompleteSearchByCustomAnalyzer(indexName, key, size: size, market: market);
            
            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
    }
}
