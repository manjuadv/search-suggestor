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

        public IEnumerable<PropertyItem> AutoCompleteSearchByCustomAnalyzer(string indexName, string text, string market = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyItem> AutoCompleteSearchSimple(string indexName, string text, string market = null, int size=10)
        {
            if (market == null)
            {
                var results = elasticClient.Search<PropertyItem>(s => s
                .Index(indexName)
                .Size(size)
                //.Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.Name).Query(text))));
                //.Query(q => q.MultiMatch(m => m.Fields(f => f.Fields("name", "formerName", "city")).Query(text))));
                /*.Query(q => q.Bool(b => b
                   .Must(mu => mu
                               .MultiMatch(m => m
                                       .Fields(f => f.Field("Name").Field("FormerName").Field("City"))
                                 .Query(text)
                                  ) && q
                                .MultiMatch(m => m
                            .Fields(f => f.Field("Name").Field("FormerName").Field("City"))
                                   .Query("Califronia")
                                )))));*/
                //.Query(q => q.MultiMatch(mm => mm.Query(text))));
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
               //.Analyzer("standard")
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
