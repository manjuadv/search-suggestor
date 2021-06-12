﻿using Nest;
using Newtonsoft.Json;
using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors.Aws
{
    public class AwsElasticsearchSetupConnector : ISetupConnector<PropertyItem>
    {
        private readonly IElasticClient elasticClient;

        public AwsElasticsearchSetupConnector(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public void CreateSearchSuggetionIndex(string indexName)
        {
            Func<CreateIndexDescriptor, ICreateIndexRequest> createIndexReqeust = i => i
            .Settings(st=>st
            .Analysis(an=>an
                .TokenFilters(tf=>tf.EdgeNGram("edge_ng_filter", eg=>eg.MinGram(1).MaxGram(20)))
                .Analyzers(an=>an.Custom("autocomp_cust_no_stop", c=>c.Tokenizer("standard").Filters("lowercase", "edge_ng_filter", "stop")))                
            )).Map<PropertyItem>(mm => mm
        //.AutoMap()
        .Properties(p => p
            .Text(t => t
                .Name(n => n.Name)
                .Analyzer("autocomp_cust_no_stop")
            )
            .Text(t => t
                .Name(n => n.FormerName)
                .Analyzer("autocomp_cust_no_stop")
            )

            ));


            var createIndexResponse = elasticClient.Indices.Create(indexName, createIndexReqeust);
        }
        
        public void CreateIndexAutoMap(string indexName)
        {
            var createIndexResponse = elasticClient.Indices.CreateAsync(indexName, c => c
           .Map<PropertyItem>(m => m.AutoMap()));
        }

        public void IndexRecord(PropertyItem proerpty, string indexName)
        {
            var response = elasticClient.Index(proerpty, i => i.Index(indexName));
        }

        public void IndexRecordsBulk(IEnumerable<PropertyItem> propertis, string indexName)
        {
            var response = elasticClient.Bulk(b => b.Index(indexName).IndexMany(propertis));
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
            }
        }

        public void IndexRecordsBulkAll(IEnumerable<PropertyItem> propertis, string indexName, int itemsPerRequest)
        {
            var bulkAllObservable = elasticClient.BulkAll(propertis, b => b
            .Index(indexName)
            .BackOffTime("30s")
            .BackOffRetries(2)
            .RefreshOnCompleted()
            .MaxDegreeOfParallelism(Environment.ProcessorCount)
            .Size(itemsPerRequest))
            .Wait(TimeSpan.FromMinutes(15), next =>
            {

            });
        }
    }
}
