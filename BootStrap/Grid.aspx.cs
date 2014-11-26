using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.Data;


namespace BootStrap
{
    public partial class Grid : System.Web.UI.Page
    {
        XElement xE;

        public string XMLDataSource { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public string Params { get; set; }
        public string DataSourceID { get; set; }
        public string Subtitle { get; set; }
        public string Target { get; set; }
        GridView grv = new GridView();

        protected void Page_Load(object sender, EventArgs e)
        {
            XMLDataSource = "App_Data/DSBGrids.xml";
            int Id = (Request.QueryString["Id"] != null ? int.Parse(Request.QueryString["Id"].ToString()) : 0);
            int sId = (Request.QueryString["SubId"] != null ? int.Parse(Request.QueryString["SubId"].ToString()) : 0);
            int width = (Request.QueryString["width"] != null ? int.Parse(Request.QueryString["width"].ToString()) : 0);
            int height = (Request.QueryString["height"] != null ? int.Parse(Request.QueryString["height"].ToString()) : 0);
            showGrid(Id, width, height, sId);
        }

        private void showGrid(int Id, int width, int height, int SubId)
        {
            grv = GridView1;
            DataTable dt = new DataTable();
            readGridXML(Id, SubId);
            if (Name == null)
            {
                xE = null;
                return;
            }
            dt = readGridData();
            grv.DataSource = dt;
            grv.DataBind();
            grv.Width = width;
            grv.Height = height;
           
        }

        private DataTable readGridData()
        {
            DataTable dt = new DataTable();
            singleton s = new singleton();
            dt = s.SelectDTQuery(Query);
            return dt;
        }


        private void readGridXML(int Id, int SubId)
        {
            // Open Source if not already openned
            if (xE == null) xE = XElement.Load(HttpContext.Current.Server.MapPath("~/" + XMLDataSource));
            // Read Chart Definition
            if (Id == 0) return;
            var q = from ll in xE.Descendants("Query")
                    where (ll.Parent.Attribute("ID").Value == Id.ToString())
                    select new
                    {
                        DSid = ll.Attribute("DataSourceID").Value,
                        nam = ll.Parent.Attribute("Name").Value,
                        Subt = (ll.Parent.Attribute("SubTitle") == null ? "" : ll.Parent.Attribute("SubTitle").Value.ToString()),
                        Query = (ll != null ? ll.Value : ""),                                      
                        Trgt = (ll.Attribute("Target") != null ? ll.Attribute("Target").Value : "")
                    };
            foreach (var l in q)
            {
                Name = l.nam;
                Query = (l.Query == null ? "" : l.Query);               
                DataSourceID = (l.DSid == null ? "" : l.DSid);
                Subtitle = l.Subt;               
                Target = l.Trgt;
                break;
            }
        }

    }
}