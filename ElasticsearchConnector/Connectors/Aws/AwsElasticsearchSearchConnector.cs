using Nest;
using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors.Aws
{
    public class AwsElasticsearchSearchConnector : ISearchConnector<PropertyItem>
    {
        private readonly IElasticClient elasticClient;

        public AwsElasticsearchSearchConnector(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public IEnumerable<PropertyItem> AutoCompleteSearchByCompletionSuggester(string indexName, string text, string market = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyItem> AutoCompleteSearchByCustomAnalyzer(string indexName, string text, string market = null, int size = 10)
        {
            if (market == null)
            {
                var searchResult = elasticClient.Search<PropertyItem>(s => s
                    .Index(indexName)
                    .Size(size)
                    .Query(q => q
                        .DisMax(dm=>dm
                            .Queries(dq=>dq
                                .Match(m => m
                                    .Boost(2) // give weight so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    //.Fuzziness(Fuzziness.EditDistance(2))
                                    .Query(text)
                                ),
                                dq=> dq.Match(m => m
                                    .Boost(1)
                                    .Analyzer("standard")
                                    .Field(f => f.StreetAddress)
                                    //.Fuzziness(Fuzziness.EditDistance(2))
                                    .Query(text)
                                )
                            )
                        )                      
                    )
                );

                return searchResult?.Documents;
            }
            else
            {
                var searchResult = elasticClient.Search<PropertyItem>(s => s
                    .Index(indexName)
                    .Size(size)
                    .Query(q => q
                        .DisMax(dm => dm
                            .Queries(dq => dq
                                .Match(m => m
                                    .Boost(2) // give weight so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    //.Fuzziness(Fuzziness.EditDistance(2))
                                    .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(1) 
                                     .Analyzer("standard")
                                     .Field(f => f.StreetAddress)
                                     //.Fuzziness(Fuzziness.EditDistance(2))
                                     .Query(text)
                                )
                            )
                        )
                        && q.Term(t => t
                            .Field(f => f.Market)
                            .Value(market.ToLower())
                        )
                    )
                );

                return searchResult?.Documents;
            }
        }
        
    }
}
