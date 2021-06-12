﻿using Elasticsearch.Net;
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


            //BasicAuthenticationCredentials auth = new BasicAuthenticationCredentials("propuser", "data@1User");

            Uri url = new Uri("https://propuser:data$1User@search-properties-fy3tslyamx44nmky4ezpycpaea.ap-south-1.es.amazonaws.com");
            ConnectionSettings settings = new ConnectionSettings(url);
            IElasticClient elasticClient = new ElasticClient(settings);
            
           
            string indexName = "property1";

            //CreateIndexAutoMap(elasticClient, indexName);

            //IndexPropertyItem(elasticClient, indexName, propertyList[0]);
            //SearchMatchPrase(elasticClient, indexName, "Abilene");
            //SearchTerm(elasticClient, indexName, "Abilene");
            //SearchMatchPrase(elasticClient, indexName, "Cur"); // No match


            //SearchMatchPrefixPhase(elasticClient, indexName, "Ranc");

            //IndexPropertyItemBulkAll(elasticClient, indexName, propertyList, 10);

            //SearchMatchPrefixPhase(elasticClient, indexName, "Stone R", 100, null);//"Atlanta");
            SearchMatchPrefixPhase(elasticClient, indexName, "Stone vi", 100, null);//"Atlanta");

        }
        private static void CreateIndexAutoMap(IElasticClient elasticClient, string indexName)
        {
            /*var createIndexResponse = elasticClient.Indices.CreateAsync(indexName, c => c
            .Map<PropertyItem>(m => m.AutoMap()));*/
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.CreateIndex(indexName);
        }
        private static void IndexPropertyItem(IElasticClient elasticClient, string indexName, PropertyItem proerpty)
        {
            /*var response = elasticClient.Index(proerpty, i => i.Index(indexName));
            Console.WriteLine(JsonConvert.SerializeObject(response.Result));*/
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.IndexRecord(proerpty, indexName);
        }
        private static void IndexPropertyItemBulkAll(IElasticClient elasticClient, string indexName, List<PropertyItem> propertis, int itemsPerRequest)
        {
            /*var bulkAllObservable = elasticClient.BulkAll(propertis, b => b
            .Index(indexName)
            .BackOffTime("30s")
            .BackOffRetries(2)
            .RefreshOnCompleted()
            .MaxDegreeOfParallelism(Environment.ProcessorCount)
            .Size(itemsPerRequest))
            .Wait(TimeSpan.FromMinutes(15), next =>
            {
                
            });*/
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.IndexRecordsBulkAll(propertis, indexName, itemsPerRequest);
        }
        private static void IndexPropertyItemBulk(IElasticClient elasticClient, string indexName, List<PropertyItem> propertis)
        {
            /*var response = elasticClient.Bulk(b => b.Index(indexName).IndexMany(propertis));
            if (response.Errors)
            {
                foreach (var errorItem in response.ItemsWithErrors)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(errorItem));
                    Console.WriteLine(string.Format("Error happed while uploading property data bulk.", response.OriginalException));
                }                
            }
            else
            {
                foreach (var responseItem in response.Items)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(responseItem));
                }                
            }*/
            AwsElasticsearchSetupConnector setupConnector = new AwsElasticsearchSetupConnector(elasticClient);
            setupConnector.IndexRecordsBulk(propertis, indexName);
        }

        private static void SearchMatchPrase(IElasticClient elasticClient, string indexName, string key)
        {
            /*var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.Match(m => m.Field(f => f.Name).Query(key)))) ;*/

            AwsElasticsearchSearchConnector searchConnector = new AwsElasticsearchSearchConnector(elasticClient);
            IEnumerable<PropertyItem> results = searchConnector.SearchByName(indexName, key);
            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
        private static void SearchTerm(IElasticClient elasticClient, string indexName, string key)
        {
            /*key = key.ToLower();


            var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.Term(t => t.Field(f => f.Market).Value(key))));*/
            AwsElasticsearchSearchConnector searchConnector = new AwsElasticsearchSearchConnector(elasticClient);
            IEnumerable<PropertyItem> results = searchConnector.FilterByName(indexName, key);

            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
        private static void SearchMatchPrefixPhase(IElasticClient elasticClient, string indexName, string key, int size, string market)
        {
            /*var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.Name).Query(key))));*/
            AwsElasticsearchSearchConnector searchConnector = new AwsElasticsearchSearchConnector(elasticClient);
            IEnumerable<PropertyItem> results = searchConnector.AutoCompleteSearchSimple(indexName, key, size: size, market:market);

            foreach (var result in results)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }
    }
}
