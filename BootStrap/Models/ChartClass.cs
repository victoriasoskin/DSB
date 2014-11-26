using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootStrap.Models
{
    public class ChartClass
    {
        public int  ID { get; set; }
        public string Name { get; set; }
        public string SubId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ChartType { get; set; }
        public string Target { get; set; }
        public string Query { get; set; }
        public string URL { get; set; }
        public string LegandLabels { get; set; }
        public string XLables { get; set; }
        public string sYvalues { get; set; }
        public string DataSourceId { get; set; }
        //styles
        public string Entity { get; set; }
        public string Member { get; set; }
        public string Enum { get; set; }
        public string Property { get; set; }
        public string PropertyType { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        
    }
}