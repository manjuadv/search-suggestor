using SmartApart.Core;
using SmartApart.Utils;
using System;
using System.Collections.Generic;

namespace Data_Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFileName = "properties.json";
            string filePath = @"..\..\..\..\DataFiles\";

            List<string> objectStrings = JsonReadHelper.GetObjectString(filePath + sourceFileName);
            List<PropertyItem> propertyList = JsonReadHelper.GetPropertyModelByJsonStrings(objectStrings, sourceFileName);
        }
    }
}
