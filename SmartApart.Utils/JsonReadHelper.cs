using SmartApart.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartApart.Utils
{
    public static class JsonReadHelper
    {
        private const string OBJECT_IDENTIFIER_NAME = "property";
        private const string COLON = ":";
        private const string COMMA = ",";
        private const string QUATATION = "\"";
        private const string OPEN_CURLY_BRACKET = "{";
        private const string CLOSE_CURLY_BRACKET = "}";
        private const string UPLOADED_PROPERTY_ID_FILE = "uploaded_property_IDs.txt";

        private static readonly Regex whiteSpaceSearch = new Regex(@"\s+");

        public static List<string> GetObjectString(string filePath)
        {
            string fileContent = System.IO.File.ReadAllText(filePath);
            List<string> objStringList = new List<string>();
            int startIndex = 0;
            while (fileContent.IndexOf(OBJECT_IDENTIFIER_NAME, startIndex) > 0)
            {
                int identifierEndIndex = fileContent.IndexOf(OBJECT_IDENTIFIER_NAME, startIndex);
                int openBracketIndex = fileContent.IndexOf(OPEN_CURLY_BRACKET, identifierEndIndex);
                int closeBracketIndex = fileContent.IndexOf(CLOSE_CURLY_BRACKET, openBracketIndex);

                if (identifierEndIndex > 0 && openBracketIndex > 0 && closeBracketIndex > 0)
                {
                    string objString = fileContent.Substring(openBracketIndex, (closeBracketIndex - openBracketIndex + 1));
                    objStringList.Add(objString.Replace(Environment.NewLine, string.Empty));
                }
                startIndex = closeBracketIndex + 1;
                if (objStringList.Count > 499)
                    break;
            }
            return objStringList;
        }
        public static List<PropertyItem> GetPropertyModelByJsonStrings(List<string> jsonStringList, string sourceFileName, bool allowDuplicateUpload=false)
        {
            List<PropertyItem> objectList = new List<PropertyItem>();
            Dictionary<long, int> uploaedPropertyIDs = GetUploadedPropertyIDs(sourceFileName);
            for (int i = 0; i < jsonStringList.Count; i++)
            {
                string jsonString = jsonStringList[i];
                PropertyItem propertyItem = null;
                try
                {
                    propertyItem = Newtonsoft.Json.JsonConvert.DeserializeObject<PropertyItem>(jsonString);
                }
                catch (Exception ex)
                {
                    propertyItem = GetPropertyItemByJsonString(jsonString);
                }

                if (propertyItem == null)
                {
                    Console.WriteLine(string.Format("Error happened while reading record {0}",i));
                    continue;
                }

                if (uploaedPropertyIDs.Keys.Contains(i))
                {
                    if (allowDuplicateUpload)
                    {
                        objectList.Add(propertyItem);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Record {0} already uploaded", i));
                    }
                }
                else
                {
                    objectList.Add(propertyItem);
                    uploaedPropertyIDs.Add(i, 1);
                    SaveUploadedPropertyID(sourceFileName, i);
                }
            }
            return objectList;
        }
        private static PropertyItem GetPropertyItemByJsonString(string jsonString)
        {
            var propertyItem = new PropertyItem { };
            PropertyInfo[] properties = typeof(PropertyItem).GetProperties();
            foreach (var propertyInfo in properties)
            {
                string jsonPropertyName = Char.ToLower(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1);
                int propertyStartIndex = jsonString.IndexOf(jsonPropertyName);
                int colonIndex = jsonString.IndexOf(COLON, propertyStartIndex);
                int commaIndex = jsonString.IndexOf(COMMA, colonIndex);
                if (propertyInfo.Name == "PropertyID")
                {
                    // valString = jsonString.Substring(colonIndex + 1, (commaIndex - colonIndex -1)).Trim();
                    string valueNoWhiteSpace = whiteSpaceSearch.Replace(jsonString.Substring(colonIndex + 1, (commaIndex - colonIndex - 1)), string.Empty);

                    long val = long.Parse(valueNoWhiteSpace);
                    propertyItem.PropertyID = val;
                }
                else if (propertyInfo.Name == "Lat")
                {
                    if (commaIndex > 0)
                    {
                        string valString = jsonString.Substring(colonIndex + 1, (commaIndex - colonIndex - 1)).Trim();
                        propertyItem.Lat = valString.ToConvertedDouble();
                    }
                    else
                    {
                        int closeBracketIndex = jsonString.IndexOf(CLOSE_CURLY_BRACKET, colonIndex);
                        string valString = jsonString.Substring(colonIndex + 1, (closeBracketIndex - colonIndex - 1)).Trim();
                        propertyItem.Lat = valString.ToConvertedDouble();
                    }
                }
                else if (propertyInfo.Name == "Lng")
                {
                    if (commaIndex > 0)
                    {
                        string valString = jsonString.Substring(colonIndex + 1, (commaIndex - colonIndex - 1)).Trim();
                        propertyItem.Lng = valString.ToConvertedDouble();
                    }
                    else
                    {
                        int closeBracketIndex = jsonString.IndexOf(CLOSE_CURLY_BRACKET, colonIndex);
                        string valString = jsonString.Substring(colonIndex + 1, (closeBracketIndex - colonIndex - 1)).Trim();
                        propertyItem.Lng = valString.ToConvertedDouble();
                    }
                }
                else
                {
                    int firstQuatationIndex = jsonString.IndexOf(QUATATION, colonIndex);
                    int secondQuatationIndex = jsonString.IndexOf(QUATATION, firstQuatationIndex + 1);
                    string valString = jsonString.Substring(firstQuatationIndex + 1, (secondQuatationIndex - firstQuatationIndex - 1)).Trim();

                    if (propertyInfo.Name == "Name")
                    {
                        propertyItem.Name = valString;
                    }
                    else if (propertyInfo.Name == "FormerName")
                    {
                        propertyItem.FormerName = valString;
                    }
                    else if (propertyInfo.Name == "StreetAddress")
                    {
                        propertyItem.StreetAddress = valString;
                    }
                    else if (propertyInfo.Name == "City")
                    {
                        propertyItem.City = valString;
                    }
                    else if (propertyInfo.Name == "Market")
                    {
                        propertyItem.Market = valString;
                    }
                    else if (propertyInfo.Name == "State")
                    {
                        propertyItem.State = valString;
                    }

                }
            }
            return propertyItem;
        }
        private static double ToConvertedDouble(this string inputString)
        {
            string valueNoWhiteSpace = whiteSpaceSearch.Replace(inputString, string.Empty);
            if (valueNoWhiteSpace.IndexOf('e') > 0)
            {
                string[] valParts = valueNoWhiteSpace.Split('e');
                double decimalVal = double.Parse(valParts[0].Trim());
                int exponent = int.Parse(valParts[1].Trim());
                double convertedValue = decimalVal * Math.Pow(10, exponent);
                return convertedValue;
            }
            else
            {
                return double.Parse(inputString);
            }
        }
        private static Dictionary<long, int> GetUploadedPropertyIDs(string sourceFileName)
        {
            if (File.Exists(UPLOADED_PROPERTY_ID_FILE))
            {
                string[] idList = File.ReadAllLines(UPLOADED_PROPERTY_ID_FILE);
                Dictionary<long, int> uploadedIDList = new Dictionary<long, int>();
                foreach (string line in idList)
                {
                    string[] lineParts = line.Split(',');
                    string sequenceID = lineParts[0];
                    string fileName = lineParts[1];
                    if (fileName == sourceFileName)
                        uploadedIDList.Add(long.Parse(sequenceID), 1);
                }
                return uploadedIDList;
            }
            else
            {
                return new Dictionary<long, int>();
            }
        }
        private static void SaveUploadedPropertyID(string sourceFileName, int sequenceID)
        {
            File.AppendAllLines(UPLOADED_PROPERTY_ID_FILE, new string[] { sequenceID.ToString() + "," + sourceFileName });
        }
    }
}
