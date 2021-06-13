using Elasticsearch.Net;
using ElasticsearchConnector.Connectors.Property;
using ElasticsearchConnector.Connectors;
using Nest;
using Newtonsoft.Json;
using SmartApart.Core.Models;
using SmartApart.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticsearchConnector.Connectors.ManagementCompany;

namespace Data_Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            //MgmtMethod();
            PropertyMethod();
        }
        private static void PropertyMethod()
        {
            string sourceFileName = "properties.json";
            string filePath = @"..\..\..\..\DataFiles\";

            //List<string> objectStrings = JsonReadHelper.GetObjectString(filePath + sourceFileName);
            //List<PropertyItem> propertyList = JsonReadHelper.GetPropertyModelByJsonStrings(objectStrings, sourceFileName, allowDuplicateUpload: true);


            //Uri url = new Uri("https://propuser:data$1User@search-properties-fy3tslyamx44nmky4ezpycpaea.ap-south-1.es.amazonaws.com");
            Uri url = new Uri("http://localhost:9200/");
            ConnectionSettings settings = new ConnectionSettings(url);
            settings.EnableDebugMode();
            IElasticClient elasticClient = new ElasticClient(settings);


            string indexName = "property5";

            //CreateIndexAutoMap(elasticClient, indexName);

            //CreateIndexSearchSuggestion(elasticClient, indexName);

            //IndexPropertyItem(elasticClient, indexName, propertyList[0]);
            //SearchMatchPrase(elasticClient, indexName, "Abilene");
            //SearchTerm(elasticClient, indexName, "Abilene");
            //SearchMatchPrase(elasticClient, indexName, "Cur"); // No match


            //SearchMatchPrefixPhase(elasticClient, indexName, "Ranc");

            //IndexPropertyItemBulkAll(elasticClient, indexName, propertyList, 10);

            //SearchMatchPrefixPhase(elasticClient, indexName, "Stone R", 100, null);//"Atlanta");
            string[] marketList = new string[]{ "Francisc", "Atlanta" };
            //SearchMatchPrefixPhase(elasticClient, indexName, "mead", 20, marketList);
            SearchMatchPrefixPhase(elasticClient, indexName, "Brookfiel", 400, market:marketList);
        }
        private static void MgmtMethod()
        {
            string sourceFileName = "mgmt.json";
            string filePath = @"..\..\..\..\DataFiles\";

            //List<string> objectStrings = JsonReadHelper.GetMgmtObjectString(filePath + sourceFileName);
            //List<MgmtCompany> mgmtCompList = JsonReadHelper.GetMgmtCompanyModelByJsonStrings(objectStrings, sourceFileName, allowDuplicateUpload: false);

            string indexName = "mgmt4";
            Uri url = new Uri("http://localhost:9200/");
            ConnectionSettings settings = new ConnectionSettings(url);
            settings.EnableDebugMode();
            IElasticClient elasticClient = new ElasticClient(settings);

            IMgmtCompSetupConnector setupCon = new MgmtCompSetupConnector(elasticClient);
            //setupCon.CreateSearchSuggetionIndex(indexName);
            //setupCon.IndexRecord(mgmtCompList[0], indexName);
            //setupCon.IndexRecordsBulkAll(mgmtCompList, indexName, 50);

            /*IMgmtCompSearchConnector srchCon = new MgmtCompSearchConnector(elasticClient);
            srchCon.AutoCompleteSearchByCustomAnalyzer(indexName, "Avan", market: null, size: 100);*/

        }
        private static void CreateIndexSearchSuggestion(IElasticClient elasticClient, string indexName)
        {
            IPropertySetupConnector setupConnector = new PropertySetupConnector(elasticClient);
            setupConnector.CreateSearchSuggetionIndex(indexName);
        }
        private static void IndexPropertyItem(IElasticClient elasticClient, string indexName, PropertyItem proerpty)
        {
            IPropertySetupConnector setupConnector = new PropertySetupConnector(elasticClient);
            setupConnector.IndexRecord(proerpty, indexName);
        }
        private static void IndexPropertyItemBulkAll(IElasticClient elasticClient, string indexName, List<PropertyItem> propertis, int itemsPerRequest)
        {
            IPropertySetupConnector setupConnector = new PropertySetupConnector(elasticClient);
            setupConnector.IndexRecordsBulkAll(propertis, indexName, itemsPerRequest);
        }
        private static void IndexPropertyItemBulk(IElasticClient elasticClient, string indexName, List<PropertyItem> propertis)
        {
            IPropertySetupConnector setupConnector = new PropertySetupConnector(elasticClient);
            setupConnector.IndexRecordsBulk(propertis, indexName);
        }
        private static void SearchMatchPrefixPhase(IElasticClient elasticClient, string indexName, string key, int size, string[] market)
        {
            /*IPropertySearchConnector searchConnector = new PropertySearchConnector(elasticClient);
            //IEnumerable<PropertyItem> results = searchConnector.AutoCompleteSearchSimple(indexName, key, size: size, market:market);

            //IEnumerable<PropertyItem> results = searchConnector.AutoCompleteNameByCustomAnalyzer(indexName, key, size: size, market: market);
            IEnumerable<PropertyItem> results = searchConnector.AutoCompleteSearchByCustomAnalyzer(indexName, key, size: size, market: market, misspellingMaxAllowed:0);
            
            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }*/
        }
    }
}
