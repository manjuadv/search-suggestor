using ElasticsearchConnector.Connectors;
using ElasticsearchConnector.Connectors.ManagementCompany;
using ElasticsearchConnector.Connectors.Property;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration configuration;
        private string indexNameProperty;
        private string indexNameMgmtCompany;
        private string indexPropertySearch;
        private string indexMgmtCompanySearch;

        public ElasticsearchService(IElasticClient elasticClient, IConfiguration configuration)
        {
            this.elasticClient = elasticClient;
            this.configuration = configuration;
            ConfigureIndexes();
        }
        private void ConfigureIndexes()
        {
            var indexPropertyAutoComp = configuration["ElasticsearchSettings:IndexPropertyAutoComp"];
            if (string.IsNullOrEmpty(indexPropertyAutoComp))
                throw new Exception("Property auto-complete index not provided");
            var indexMgmtCompAutoComp = configuration["ElasticsearchSettings:IndexMgmtCompAutoComp"];
            if (string.IsNullOrEmpty(indexMgmtCompAutoComp))
                throw new Exception("Mgmt company auto-complete index not provided");
            var IndexPropertySearch = configuration["ElasticsearchSettings:IndexPropertySearch"];
            if (string.IsNullOrEmpty(IndexPropertySearch))
                throw new Exception("Property search index not provided");
            var IndexMgmtCompSearch = configuration["ElasticsearchSettings:IndexMgmtCompSearch"];
            if (string.IsNullOrEmpty(IndexMgmtCompSearch))
                throw new Exception("Mgmt company search index not provided");
            this.indexNameProperty = indexPropertyAutoComp;
            this.indexNameMgmtCompany = indexMgmtCompAutoComp;
            this.indexPropertySearch = IndexPropertySearch;
            this.indexMgmtCompanySearch = IndexMgmtCompSearch;
        }
        public IEnumerable<SearchSuggestionResult> GetAutocompleteSuggestions(string text, string[] markets = null, 
            int size = 25, int misspellingMaxAllowed=2)
        {         
            ISearchResponse<SearchEntity> searchResponse = GetSearchResults(indexNameProperty, indexNameMgmtCompany, text, size: size, market: markets, 
                misspellingMaxAllowed: misspellingMaxAllowed);
            if (searchResponse == null)
            {
                throw new Exception("Null response received");
            }
            else
            {
                List<SearchSuggestionResult> resultsList = new List<SearchSuggestionResult>();
                string sourceType = "";
                foreach (var hit in searchResponse.Hits)
                {
                    if (hit.Index == indexNameProperty)
                    {
                        sourceType = "property";                        
                    }
                    else if (hit.Index == indexNameMgmtCompany)
                    {
                        sourceType = "mgmtComp";
                        
                    }
                    resultsList.Add(new SearchSuggestionResult { EntityID = hit.Source.PropertyID, Suggestion = hit.Source.Name, 
                        SourceType = sourceType, Rank = hit.Score, FormerName = hit.Source.FormerName, Source=hit.Source });
                }
                return resultsList;
            }
        }
        private ISearchResponse<SearchEntity> GetSearchResults(string indexProp, string indexMgmt, string text, int misspellingMaxAllowed = 0, 
            string[] market = null, int size = 10)
        {
            if (market == null)
            {
                var searchResult = elasticClient.Search<SearchEntity>(s => s
                    .Index(new string[] { indexProp, indexMgmt })
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

                var searchResult = elasticClient.Search<SearchEntity>(s => s
                    .Index(new string[] { indexProp, indexMgmt })
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

        public IEnumerable<SearchSuggestionResult> GetSearchResults(string text, string[] markets = null, int size = 25, int misspellingMaxAllowed = 2)
        {
            ISearchResponse<SearchEntity> searchResponse = GetSearchResults(indexPropertySearch, indexMgmtCompanySearch, text, size: size, market: markets,
                misspellingMaxAllowed: misspellingMaxAllowed);
            if (searchResponse == null)
            {
                throw new Exception("Null response received");
            }
            else
            {
                List<SearchSuggestionResult> resultsList = new List<SearchSuggestionResult>();
                string sourceType = "";
                foreach (var hit in searchResponse.Hits)
                {
                    if (hit.Index == indexPropertySearch)
                    {
                        sourceType = "property";
                    }
                    else if (hit.Index == indexMgmtCompanySearch)
                    {
                        sourceType = "mgmtComp";

                    }
                    resultsList.Add(new SearchSuggestionResult
                    {
                        EntityID = hit.Source.PropertyID,
                        Suggestion = hit.Source.Name,
                        SourceType = sourceType,
                        Rank = hit.Score,
                        FormerName = hit.Source.FormerName,
                        Source = hit.Source
                    });
                }
                return resultsList;
            }
        }
    }
}
