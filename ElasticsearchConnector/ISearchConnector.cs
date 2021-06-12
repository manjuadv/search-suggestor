﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector
{
    public interface ISearchConnector<T>
    {
        /// <summary>
        /// Autocomplete suggestions supposed to given with built in Completion Suggester of Elasticsearch.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        IEnumerable<T> AutoCompleteSearchByCompletionSuggester(string indexName, string text, string market = null);
        /// <summary>
        /// Autocomplete suggtions given based on a custom analyzer.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        IEnumerable<T> AutoCompleteSearchByCustomAnalyzer(string indexName, string text, string market = null);
        /// <summary>
        /// This is simple mechanism, not Stop words removing.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        IEnumerable<T> AutoCompleteSearchSimple(string indexName, string text, string market = null, int size=10);
        IEnumerable<T> SearchByName(string indexName, string text);
        IEnumerable<T> FilterByName(string indexName, string text);
    }
}