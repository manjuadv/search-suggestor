using Nest;
using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors.ManagementCompany
{
    public class MgmtCompSearchConnector : IMgmtCompSearchConnector
    {
        private readonly IElasticClient elasticClient;

        public MgmtCompSearchConnector(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public IEnumerable<MgmtCompany> AutoCompleteSearchByCompletionSuggester(string indexName, string text, string market = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MgmtCompany> AutoCompleteSearchByCustomAnalyzer(string indexName, string text, int misspellingMaxAllowed = 0, string[] market = null, int size = 10)
        {
            if (market == null)
            {
                var searchResult = elasticClient.Search<MgmtCompany>(s => s
                    .Index(indexName)
                    .Size(size)
                    .Query(q => q
                        .DisMax(dm => dm
                            .Queries(dq => dq
                                .Match(m => m
                                    .Boost(3) // give weight so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    .Fuzziness(Fuzziness.EditDistance(misspellingMaxAllowed))
                                    .Query(text)
                                ),
                                dq => dq.Match(m => m
                                     .Boost(2)
                                     .Analyzer("standard")
                                     .Field(f => f.Market)
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

                var searchResult = elasticClient.Search<MgmtCompany>(s => s
                    .Index(indexName)
                    .Size(size)
                    .Query(q => q
                        .DisMax(dm => dm
                            .Queries(dq => dq
                                .Match(m => m
                                    .Boost(2) // give weight so that match with name comes top
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
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
                        && q.Term(t => t
                            .Field(f => f.Market)
                            .Value(marketList)
                        )
                    )
                );

                return searchResult?.Documents;
            }
        }
    }
}
