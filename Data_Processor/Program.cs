using Elasticsearch.Net;
using ElasticsearchConnector.Connectors.Property;
using ElasticsearchConnector.Connectors;
using Nest;
using Newtonsoft.Json;
using SmartApart.Core.Models;
using SmartApart.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticsearchConnector.Connectors.ManagementCompany;

namespace Data_Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri url = new Uri("https://propuser:data$1User@search-properties-fy3tslyamx44nmky4ezpycpaea.ap-south-1.es.amazonaws.com");
            //Uri url = new Uri("http://localhost:9200/");
            ConnectionSettings settings = new ConnectionSettings(url);
            settings.EnableDebugMode();
            IElasticClient elasticClient = new ElasticClient(settings);

            IPropertySetupConnector setupConnectorProperty = new PropertySetupConnector(elasticClient);
            IMgmtCompSetupConnector setupConnectorMgmtComp = new MgmtCompSetupConnector(elasticClient);
            IndexingFacade indexingFacade = new IndexingFacade(setupConnectorProperty, setupConnectorMgmtComp);

            string indexPropFulltext = "prop_fulltext";
            string indexMgmtFulltext = "mgmt_fulltext";
            string indexPropAutoComp = "prop_autocomp";
            string indexMgmtAutoComp = "mgmt_autocomp";

            CreateIndices(indexingFacade, indexPropFulltext, indexMgmtFulltext, indexPropAutoComp, indexMgmtAutoComp);
            UploadProperties(indexingFacade, indexPropFulltext, indexPropAutoComp);
            UploadMgmtCompaies(indexingFacade, indexMgmtFulltext, indexMgmtAutoComp);
        }
        private static void CreateIndices(IndexingFacade indexingFacade, string indexPropFulltext, string indexMgmtFulltext, string indexPropAutoComp
            , string indexMgmtAutoComp)
        {
            indexingFacade.SetupIndices(indexPropFulltext, indexMgmtFulltext, indexPropAutoComp, indexMgmtAutoComp);
        }
        private static void UploadProperties(IndexingFacade indexingFacade, string indexPropFulltext, string indexPropAutoComp)
        {
            string sourceFileName = "properties.json";
            string filePath = @"..\..\..\..\DataFiles\";

            List<string> objectStrings = JsonReadHelper.GetObjectString(filePath + sourceFileName);
            List<PropertyItem> propertyList = JsonReadHelper.GetPropertyModelByJsonStrings(objectStrings, sourceFileName, allowDuplicateUpload: true);

            indexingFacade.UploadDataProperty(propertyList, indexPropFulltext, indexPropAutoComp);
        }
        private static void UploadMgmtCompaies(IndexingFacade indexingFacade, string indexMgmtFulltext, string indexMgmtAutoComp)
        {
            string sourceFileName = "mgmt.json";
            string filePath = @"..\..\..\..\DataFiles\";

            List<string> objectStrings = JsonReadHelper.GetMgmtObjectString(filePath + sourceFileName);
            List<MgmtCompany> mgmtCompList = JsonReadHelper.GetMgmtCompanyModelByJsonStrings(objectStrings, sourceFileName, allowDuplicateUpload: false);

            indexingFacade.UplaodDataMgmtComp(mgmtCompList, indexMgmtFulltext, indexMgmtAutoComp);
        }
    }
}
