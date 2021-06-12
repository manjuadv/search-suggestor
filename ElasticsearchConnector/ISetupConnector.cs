using Nest;
using System;
using System.Collections.Generic;

namespace ElasticsearchConnector
{
    public interface ISetupConnector<T>
    {
        void CreateIndex(string indexName);
        void IndexRecord(T document, string indexName);
        void IndexRecordsBulkAll(IEnumerable<T> documents, string indexName, int itemsPerRequest);
        void IndexRecordsBulk(IEnumerable<T> documents, string indexName);
    }
}
