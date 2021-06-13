using Nest;
using Newtonsoft.Json;
using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors.ManagementCompany
{
    public class MgmtCompSetupConnector : IMgmtCompSetupConnector
    {
        private readonly IElasticClient elasticClient;

        public MgmtCompSetupConnector(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public void CreateFulltextSearchIndex(string indexName)
        {
            Func<CreateIndexDescriptor, ICreateIndexRequest> createIndexReqeust = i => i
            .Settings(st => st
                .Analysis(an => an
                    .Analyzers(an => an
                        .Custom("cust_no_stop", c => c
                            .Tokenizer("standard").Filters("lowercase", "stop")
                         )
                     )
                 )
             )
            .Map<MgmtCompany>(mm => mm
                .Properties(p => p
                    .Text(t => t
                        .Name(n => n.Name)
                        .Analyzer("cust_no_stop")
                    )
                    .Text(t => t
                        .Name(n => n.Market)
                        .Analyzer("cust_no_stop")
                    )
                    .Text(t => t
                        .Name(n => n.State)
                        .Analyzer("standard")
                    )
                )
              );


            var createIndexResponse = elasticClient.Indices.Create(indexName, createIndexReqeust);
        }
        public void CreateSearchSuggetionIndex(string indexName)
        {
            Func<CreateIndexDescriptor, ICreateIndexRequest> createIndexReqeust = i => i
            .Settings(st => st
                .Analysis(an => an
                    .TokenFilters(tf => tf
                        .EdgeNGram("edge_ng_filter", eg => eg
                            .MinGram(1).MaxGram(20)
                         )
                     )
                    .Analyzers(an => an
                        .Custom("autocomp_cust_no_stop", c => c
                            .Tokenizer("standard").Filters("lowercase", "edge_ng_filter", "stop")
                         )
                     )
                 )
             )
            .Map<MgmtCompany>(mm => mm
                .Properties(p => p
                    .Text(t => t
                        .Name(n => n.Name)
                        .Analyzer("autocomp_cust_no_stop")
                    )
                    .Text(t => t
                        .Name(n => n.Market)
                        .Analyzer("autocomp_cust_no_stop")
                        //.Fields(f=>f.Keyword(k=>k.Name(n=>n.Market).IgnoreAbove(256)))
                    )
                    .Text(t => t
                        .Name(n => n.State)
                        .Analyzer("standard")
                    )
                )
              );


            var createIndexResponse = elasticClient.Indices.Create(indexName, createIndexReqeust);
        }

        public void IndexRecord(MgmtCompany mgmtCompany, string indexName)
        {
            var response = elasticClient.Index(mgmtCompany, i => i.Index(indexName));
        }

        public void IndexRecordsBulk(IEnumerable<MgmtCompany> mgmtCompanies, string indexName)
        {
            var response = elasticClient.Bulk(b => b.Index(indexName).IndexMany(mgmtCompanies));
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

        public void IndexRecordsBulkAll(IEnumerable<MgmtCompany> mgmtCompanies, string indexName, int itemsPerRequest)
        {

            var bulkAllObservable = elasticClient.BulkAll(mgmtCompanies, b => b
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
