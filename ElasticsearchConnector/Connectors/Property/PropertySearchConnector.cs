using Nest;
using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors.Property
{
    public class PropertySearchConnector : IPropertySearchConnector
    {
        private readonly IElasticClient elasticClient;

        public PropertySearchConnector(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public IEnumerable<PropertyItem> AutoCompleteSearchByCompletionSuggester(string indexName, string text, string market = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyItem> AutoCompleteSearchByCustomAnalyzer(string indexName, string text, int misspellingMaxAllowed = 0, string[] market = null, int size = 10)
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
                                    .Boost(6) // highest weight given to name so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                    .Query(text)
                                ),
                                dq=> dq.Match(m => m
                                    .Boost(5) // second highest weight given to name so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.FormerName)
                                    .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                    .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(4)
                                     .Analyzer("standard")
                                     .Field(f => f.City)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(3)
                                     .Analyzer("standard")
                                     .Field(f => f.Market)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(2)
                                     .Analyzer("standard")
                                     .Field(f => f.StreetAddress)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(1)
                                     .Analyzer("standard")
                                     .Field(f => f.State)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
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
                List<string> marketList = new List<string>();
                foreach (string v in market) marketList.Add(v.ToLower());

                var searchResult = elasticClient.Search<PropertyItem>(s => s
                    .Index(indexName)
                    .Size(size)
                    .Query(q => q
                        .DisMax(dm => dm
                            .Queries(dq => dq
                                .Match(m => m
                                    .Boost(6) // highest weight given to name so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                    .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(5) // second highest weight given to name so that match with name comes top
                                     .Analyzer("standard")
                                     .Field(f => f.FormerName)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(4)
                                     .Analyzer("standard")
                                     .Field(f => f.City)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                ),
                                // Market is not searched with Math since it's used to filter as a keyword
                                dq => dq.Match(m => m
                                     .Boost(2)
                                     .Analyzer("standard")
                                     .Field(f => f.StreetAddress)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(1)
                                     .Analyzer("standard")
                                     .Field(f => f.State)
                                     .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                     .Query(text)
                                )
                            )
                        )
                        && q.Terms(t=>t
                            .Field(f=>f.Market)
                            .Terms(marketList)
                        )
                    )
                );

                return searchResult?.Documents;
            }
        }
        
    }
}
