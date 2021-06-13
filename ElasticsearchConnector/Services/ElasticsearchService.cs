using ElasticsearchConnector.Connectors;
using ElasticsearchConnector.Connectors.ManagementCompany;
using ElasticsearchConnector.Connectors.Property;
using Nest;
using SmartApart.Core.Models;
using SmartApart.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Services
{
    public class ElasticsearchService : ISearchService
    {
        private readonly IElasticClient elasticClient;
        private readonly string indexNameProperty;
        private readonly string indexNameMgmtCompany;

        public ElasticsearchService(IElasticClient elasticClient, string indexNameProperty, string indexNameMgmtCompany)
        {
            this.elasticClient = elasticClient;
            this.indexNameProperty = indexNameProperty;
            this.indexNameMgmtCompany = indexNameMgmtCompany;
        }
        public IEnumerable<SearchSuggestionResult> GetAutocompleteSuggestions(string text, string[] markets = null)
        {         
            ISearchResponse<PropertyItem> searchResponse = GetSearchResults(text, size: 100, market: markets, misspellingMaxAllowed: 0);
            if (searchResponse == null)
            {
                throw new Exception("Null response received");
            }
            else
            {
                List<SearchSuggestionResult> resultsList = new List<SearchSuggestionResult>();

                foreach (var hit in searchResponse.Hits)
                {
                    if (hit.Index == indexNameProperty)
                    {
                        resultsList.Add(new SearchSuggestionResult { EntityID = hit.Source.PropertyID, Suggestion = hit.Source.Name, SourceType = "property", Rank = hit.Score });
                    }
                    else if (hit.Index == indexNameMgmtCompany)
                    {
                        resultsList.Add(new SearchSuggestionResult { EntityID = hit.Source.PropertyID, Suggestion = hit.Source.Name, SourceType = "mgmtComp", Rank = hit.Score });
                    }
                }
                return resultsList;
            }
        }
        private ISearchResponse<PropertyItem> GetSearchResults(string text, int misspellingMaxAllowed = 0, 
            string[] market = null, int size = 10)
        {
            if (market == null)
            {
                var searchResult = elasticClient.Search<PropertyItem>(s => s
                    .Index(new string[] { indexNameProperty, indexNameMgmtCompany })
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

                return searchResult;
            }
            else
            {
                List<string> marketList = new List<string>();
                foreach (string v in market) marketList.Add(v.ToLower());

                var searchResult = elasticClient.Search<PropertyItem>(s => s
                    .Index(new string[] { indexNameProperty, indexNameMgmtCompany })
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
                        && q.Terms(t => t
                            .Field(f => f.Market)
                            .Terms(marketList)
                        )
                    )
                );

                return searchResult;
            }
        }
    }
}
