using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
namespace BootStrap.Models
{
    public class readXMLModel
    {
        ChartClass chartE = new ChartClass();
        public ChartClass readXMLFile(XDocument xdoc,int PermissionId,string SubId) 
        { 
            var test = xdoc;
            var chartList = from node in xdoc.Descendants()
                        where node.Name == "Chart"
                        select node;
            foreach (var element in chartList)
            {
                ParseChartData(element, PermissionId,SubId);
            }
            return chartE;
        }   


        private void ParseChartData(XElement chart,int chartId,string SubId)
        {
            if (int.Parse(chart.Attribute("ID").Value) == chartId && (chart.Attribute("SubID").Value == ""))
            {
                chartE.ID = int.Parse(chart.Attribute("ID").Value);
                chartE.SubId = chart.Attribute("SubID").Value;

                var queryDetails = from node in chart.Descendants()
                                   where node.Name == "Query"
                                   select node;
                foreach (var element in queryDetails)
                {
                    ParseQueryData(element);
                }
                var chartStyles = from node in chart.Descendants()
                                  where node.Name == "Style"
                                  select node;
                foreach (var element in chartStyles)
                {
                    ParseChartStyle(element);
                }
            }
           
        }

        private void ParseChartStyle(XElement element)
        {
            chartE.Entity = element.Attribute("Entity").Value;
            chartE.Member = element.Attribute("Member").Value;
            chartE.Enum = element.Attribute("Enum").Value;
            chartE.Property = element.Attribute("Property").Value;
            chartE.PropertyType = element.Attribute("PropertyType").Value;
            chartE.DataType = element.Attribute("DataType").Value;
            chartE.Value = element.Attribute("Value").Value; 
        }

        private void ParseQueryData(XElement element)
        {
            chartE.ChartType = element.Attribute("ChartType").Value;
            chartE.DataSourceId = element.Attribute("DataSourceID").Value;
            chartE.Target = element.Attribute("Target").Value;
            chartE.XLables = element.Attribute("XLabels").Value;
            chartE.Query = element.Value; //the select query 
        }


    }
   
}