using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors
{
    public class IndexingFacade
    {
        private readonly IPropertySetupConnector propertySetupConnector;
        private readonly IMgmtCompSetupConnector mgmtCompSetupConnector;

        public IndexingFacade(IPropertySetupConnector propertySetupConnector, IMgmtCompSetupConnector mgmtCompSetupConnector)
        {
            this.propertySetupConnector = propertySetupConnector;
            this.mgmtCompSetupConnector = mgmtCompSetupConnector;
        }

        public void SetupIndices(string propFulltextIndex, string mgmtFulltextIndex, string propAutoCompIndex, string mgmtAutoCompIndex)
        {
            this.SetupIndexFullTextSearch(propFulltextIndex, mgmtFulltextIndex);
            this.SetupIndexAutocomplete(propAutoCompIndex, mgmtAutoCompIndex);
        }
        private void SetupIndexFullTextSearch(string indexPropFulltext, string indexMgmtFulltext)
        {
            propertySetupConnector.CreateFulltextSearchIndex(indexPropFulltext);
            mgmtCompSetupConnector.CreateFulltextSearchIndex(indexMgmtFulltext);
        }
        private void SetupIndexAutocomplete(string indexPropAutocomp, string indexMgmtAutocomp)
        {
            propertySetupConnector.CreateSearchSuggetionIndex(indexPropAutocomp);
            mgmtCompSetupConnector.CreateSearchSuggetionIndex(indexMgmtAutocomp);
        }
        public void UploadDataProperty(IEnumerable<PropertyItem> data, string indexPropFulltext, string propAutoCompIndex, int itemsPerRequest = 100)
        {
            propertySetupConnector.IndexRecordsBulkAll(data, indexPropFulltext, itemsPerRequest);
            propertySetupConnector.IndexRecordsBulkAll(data, propAutoCompIndex, itemsPerRequest);
        }
        public void UplaodDataMgmtComp(IEnumerable<MgmtCompany> data, string mgmtFulltextIndex, string indexMgmtAutocomp, int itemsPerRequest = 100)
        {
            mgmtCompSetupConnector.IndexRecordsBulkAll(data, mgmtFulltextIndex, itemsPerRequest);
            mgmtCompSetupConnector.IndexRecordsBulkAll(data, indexMgmtAutocomp, itemsPerRequest);
        }
    }
}
