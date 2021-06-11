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

            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200/"));
            IElasticClient elasticClient = new ElasticClient(settings);
            string indexName = "property1";

            //IndexPropertyItem(elasticClient, indexName, propertyList[1]);
            //SearchMatchPrase(elasticClient, "property1", "Abilene");
            //SearchTerm(elasticClient, indexName, "Abilene");
            //SearchMatchPrase(elasticClient, "property1", "Cur"); // No match
            SearchMatchPrefixPhase(elasticClient, "property1", "Cur");
            SearchMatchPrefixPhase(elasticClient, "property1", "Ranc");

            Console.ReadKey();
        }
        private static void CreateIndexAutoMap(IElasticClient elasticClient, string indexName)
        {
            var createIndexResponse = elasticClient.Indices.CreateAsync(indexName, c => c
            .Map<PropertyItem>(m => m.AutoMap()));
        }
        private static void IndexPropertyItem(IElasticClient elasticClient, string indexName, PropertyItem proerpty)
        {
            var response = elasticClient.Index(proerpty, i => i.Index(indexName));
            Console.WriteLine(JsonConvert.SerializeObject(response.Result));
        }

        private static void SearchMatchPrase(IElasticClient elasticClient, string indexName, string key)
        {
            var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.Match(m => m.Field(f => f.Name).Query(key))));

            foreach (var result in results?.Documents)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
        private static void SearchTerm(IElasticClient elasticClient, string indexName, string key)
        {
            // TODO:Apply analyzer to make lower
            key = key.ToLower();


            var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.Term(t => t.Field(f => f.Market).Value(key))));

            foreach (var result in results?.Documents)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
        private static void SearchMatchPrefixPhase(IElasticClient elasticClient, string indexName, string key)
        {
            var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.Name).Query(key))));

            foreach (var result in results?.Documents)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
    }
}
