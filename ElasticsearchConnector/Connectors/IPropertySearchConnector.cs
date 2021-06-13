using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchConnector.Connectors
{
    public interface IPropertySearchConnector : ISearchConnector<PropertyItem>
    {
    }
}
