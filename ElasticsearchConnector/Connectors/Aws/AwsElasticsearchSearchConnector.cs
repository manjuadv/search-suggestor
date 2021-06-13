﻿using Nest;
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
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    //.Fuzziness(Fuzziness.EditDistance(2))
                                    .Query(text)
                                ),
                                dq=> dq.Match(m => m
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
                                    .Analyzer("standard")
                                    .Field(f => f.Name)
                                    //.Fuzziness(Fuzziness.EditDistance(2))
                                    .Query(text)
                                ),
                                dq => dq.Match(m => m
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
        public IEnumerable<PropertyItem> AutoCompleteNameByCustomAnalyzer(string indexName, string text, string market = null, int size = 10)
        {
            if (market == null)
            {
                var searchResult = elasticClient.Search<PropertyItem>(s => s
                    .Index(indexName)
                    .Size(size)
                    .Query(q => q
                        .Match(m => m
                            .Analyzer("standard")
                            .Field(f => f.Name)
                            //.Fuzziness(Fuzziness.EditDistance(2))
                            .Query(text)
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
                        .Match(m => m
                            .Analyzer("standard")
                            .Field(f => f.Name)
                            //.Fuzziness(Fuzziness.EditDistance(2))
                            .Query(text)
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

        public IEnumerable<PropertyItem> AutoCompleteSearchSimple(string indexName, string text, string market = null, int size=10)
        {
            if (market == null)
            {
                var results = elasticClient.Search<PropertyItem>(s => s
                .Index(indexName)
                .Size(size)
                .Query(q => q
                .DisMax(dm => dm
                    .Queries(dq => dq.MatchPhrasePrefix(m => m.Field("name").Query(text))
                        , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.FormerName).Query(text))
                        , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.StreetAddress).Query(text))
                        , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.State).Query(text))
                        , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.City).Query(text))
                    )
                )));

                return results.Documents;
            }
            else
            {
                var results = elasticClient.Search<PropertyItem>(s => s
               .Index(indexName)
               .Size(size)
               .Query(q => q
               .DisMax(dm => dm
                   .Queries(dq => dq.MatchPhrasePrefix(m => m.Field("name").Query(text))
                       , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.FormerName).Query(text))
                       , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.StreetAddress).Query(text))
                       , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.State).Query(text))
                       , dq => dq.MatchPhrasePrefix(m => m.Field(f => f.City).Query(text))
                   )
               ) && q.Term(t => t.Field(f => f.Market).Value(market.ToLower()))));

                return results.Documents;
            }            
        }

        public IEnumerable<PropertyItem> FilterByName(string indexName, string text)
        {
            // TODO:Apply analyzer to make lower
            text = text.ToLower();

            var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.Term(t => t.Field(f => f.Name).Value(text))));

            return results?.Documents;
        }

        public IEnumerable<PropertyItem> SearchByName(string indexName, string text)
        {
            var results = elasticClient.Search<PropertyItem>(s => s
            .Index(indexName)
            .Query(q => q.Match(m => m.Field(f => f.Name).Query(text))));

            return results?.Documents;
        }
    }
}
