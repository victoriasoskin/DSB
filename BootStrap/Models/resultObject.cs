using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootStrap.Models
{
    public class resultObject
    {
        public int DataIdRes { get; set; }
        public int DivId { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public int SourceDataId { get; set; }
        public string SourceDivId { get; set; }
        public int TargetDataId { get; set; }
        public string TargetDivId { get; set; }
        public string  SourceDivName { get; set; }
        public string QueryToZoom { get; set; }
        public int ViewId { get; set; }


        public  string Strip(string s, string subString)
        {
            string _tableDiv ="#TableDiv";
            string _chartDiv="#Div";

            if (s.StartsWith(_tableDiv))
            {
                s = s.Replace(_tableDiv, "");
            }
            else if (s.StartsWith(_chartDiv))
            {
                s = s.Replace(_chartDiv, "");
            }
           // s = s.Replace(subString, "");

            return s;
        }
    }

    
}

           