using System;

namespace SmartApart.Core
{
    public class PropertyItem
    {
        public long PropertyID { get; set; }
        public string Name { get; set; }
        public string FormerName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
