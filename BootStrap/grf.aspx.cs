using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using BootStrap.Models;
using BootStrap.Controllers;
using BootStrap;


namespace grf
{
    public partial class grf : System.Web.UI.Page
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public string DetailedQuery { get; set; }
        public string ChrtType { get; set; }
        public string Params { get; set; }
        public string XMLDataSource { get; set; }
        public string LegendLabels { get; set; }
        public string Xlabels { get; set; }
        public string Yvalues { get; set; }
        public string DataSourceID { get; set; }
        public string Subtitle { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }

        string sEntity = "";
        string sMember = "";
        string sEnum = "";
        string sProperty = "";
        string sPropertyType = "";
        string sDataType = "";
        string sValue = "";

        List<KeyValuePair<string,string>> cLabels = new  List<KeyValuePair<string,string>>();
        List<KeyValuePair<string, string>> cSlabel = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> cValues = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> cSeries = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> cLegend = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> cUrls = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> cAxisTypes = new List<KeyValuePair<string, string>>();
        Series srs;
        XElement xE;
        string _queryString = "";
        string _detailedQuery = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            
            XMLDataSource = "App_Data/DSBCharts.xml";
            string s = Request.QueryString["ID"];
            int Id = (Request.QueryString["Id"] != null ? int.Parse(Request.QueryString["Id"].ToString()) : 0);
            int sId = (Request.QueryString["sId"] != null ? int.Parse(Request.QueryString["sId"].ToString()) : 0);
            int width = (Request.QueryString["width"] != null ? int.Parse(Request.QueryString["width"].ToString()) : 0);
            int height = (Request.QueryString["height"] != null ? int.Parse(Request.QueryString["height"].ToString()) : 0);
            int zoomed = (Request.QueryString["zoomed"] != null ? int.Parse(Request.QueryString["zoomed"].ToString()) : 0);
            int divId = (Request.QueryString["divId"] != null ? int.Parse(Request.QueryString["divId"].ToString()) : 0);
            Params = HttpUtility.HtmlDecode( (Request.QueryString["P1"] != null ? Request.QueryString["P1"].ToString() : null));
            //if (Params!=null)
            //{
            //    byte[] t = System.Text.Encoding.Default.GetBytes(Params);
            //    Params = System.Text.Encoding.UTF8.GetString(t);
            //}
            
            Params += "|"+ (Request.QueryString["P2"] != null ? Request.QueryString["P2"].ToString() : null);
            Params += "|" + (Request.QueryString["P3"] != null ? Request.QueryString["P3"].ToString() : null);
            Params += "|" + (Request.QueryString["P4"] != null ? Request.QueryString["P4"].ToString() : null);
            Params += "|" + (Request.QueryString["P5"] != null ? Request.QueryString["P5"].ToString() : null);
            Params += "|" + (Request.QueryString["P6"] != null ? Request.QueryString["P6"].ToString() : null);
            showChart(Id,width,height,zoomed,divId, sId);
        }
        protected void showChart(int Id, int width, int height, int zoomed,int divId,int SubId = 0)
        {

            // Read XML Definition of Query

            readCharXML(Id, SubId);

            // If ReportID Is empty exit

            if (Name == null)
            {
                xE = null;
                return;
            }

            // Update Query and Name With Parameters

            updateQueryParameters(Id, zoomed, divId,864);

            // Read Query From Data Base And prepare For the chart

            readChartData();

            // Add Missing points

            addMissingPoints();

            // build Chart

            Chart Chrt = Chart1;
            buildChart(Chrt,width,height,zoomed);

            // Chart PreSetting fORMATING

            ChartPreSettings(ChrtType);

            // Chart Post Setting Formating

            ChartPostSettings(Id, SubId);
            

        }

        private void saveQuerString(int divId, int Id, int UserId, string _queryString,string _DetailedQuery)
        {
            DataActions da = new DataActions();

            if (divId==0)
            {
                divId= da.GetDivId(Id, UserId);

            }
            KeyValuePair<int, int> pair = new KeyValuePair<int,int>(divId,Id);
            if (_detailedQuery==null)
            {
                da.saveQueryString(pair, _queryString, 1);
            }
            else 
            da.saveZoomedQueryString(pair, _queryString,_DetailedQuery, 1);
        }

        private void ChartPostSettings(int Id, int SubId)
        {
            var qP = from ll in xE.Descendants("Style")
                     where (ll.Parent.Parent.Attribute("ID").Value == Id.ToString()) && (ll.Parent.Parent.Attribute("SubID").Value == SubId.ToString())
                     select new
                     {
                         Ent = ll.Attribute("Entity").Value,
                         Mbr = ll.Attribute("Member").Value,
                         Enum = ll.Attribute("Enum").Value,
                         Prop = ll.Attribute("Property").Value,
                         PropType = ll.Attribute("PropertyType").Value,
                         DataType = ll.Attribute("DataType").Value,
                         Val = ll.Attribute("Value").Value
                     };

            foreach (var l in qP)
            {
                sEntity = l.Ent;
                sMember = l.Mbr;
                sEnum = l.Enum;
                sProperty = l.Prop;
                sPropertyType = l.PropType;
                sDataType = l.DataType;
                sValue = l.Val;
                updateSettings(sEntity, sMember, sProperty, sPropertyType, sDataType, sValue, sEnum);
            }
        }

        private void updateSettings(string sEntity, string sMember, string sProperty, string sPropertyType, string sDataType, string sValue, string sEnum)
        {
            Chart chrt = Chart1;
            Series srs;
            Legend LGD = new Legend();
            switch (sEntity)
            {
                case "Chart":
                    switch (sPropertyType)
                    {
                        case "Text":
                            break;
                        case "Method":
                            switch (sMember)
                            {
                                case "Titles":
                                    switch (sProperty)
                                    {
                                        case "Docking":
                                            switch (sValue)
                                            {
                                                case "Top":
                                                    chrt.Titles[int.Parse(sEnum)].Docking = Docking.Top;
                                                    break;
                                                case "Bottom":
                                                    chrt.Titles[int.Parse(sEnum)].Docking = Docking.Bottom;
                                                    break;
                                                case "Left":
                                                    chrt.Titles[int.Parse(sEnum)].Docking = Docking.Left;
                                                    break;
                                                case "Right":
                                                    chrt.Titles[int.Parse(sEnum)].Docking = Docking.Right;
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case "Width":
                                    switch (sValue)
                                    {
                                        case "Window":
                                            if (Request.QueryString["width"] != null) chrt.Width = int.Parse(Request.QueryString["width"].ToString()) / 3 + 30;
                                            break;
                                        default:
                                            chrt.Width = Unit.Parse(sValue);
                                            break;
                                    }
                                    break;
                                case "Height":
                                    switch (sValue)
                                    {
                                        case "Window":
                                            if (Request.QueryString["Height"] != null) chrt.Height = int.Parse(Request.QueryString["Height"].ToString()) / 3 + 30;
                                            break;
                                        default:
                                            chrt.Width = Unit.Parse(sValue);
                                            break;
                                    }
                                    break;
                                case "ChartAreas":
                                    switch (sProperty)
                                    {
                                        case "Interval":
                                            chrt.ChartAreas[0].AxisX.Interval = double.Parse(sValue);
                                            break;
                                        case "Angle":
                                            Chart1.ChartAreas[0].AxisX.LabelStyle.Angle = int.Parse(sValue);
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case "Series":
                    srs = chrt.Series[int.Parse(sEnum)];
                    switch (sPropertyType)
                    {
                        case "Text":
                            srs = chrt.Series[int.Parse(sEnum)];
                            srs[sProperty] = sValue;
                            break;
                        case "Method":
                            switch (sMember)
                            {
                                default:
                                    switch (sProperty)
                                    {
                                        case "Url":
                                            srs.Url = sValue;
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case "Point":
                    break;
            }
        }

        private void ChartPreSettings(string ChrtType)
        {

            //   These in all ChartTypes

            Chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            Chart1.ChartAreas[0].Area3DStyle.Perspective = 90;
            Chart1.ChartAreas[0].Area3DStyle.IsRightAngleAxes = true;
            Chart1.ChartAreas[0].Area3DStyle.Inclination = 60;
            Chart1.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);

            switch (ChrtType)
            {
                case "Pie":
                    Chart1.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;
                    foreach (Series srs in Chart1.Series)
                        srs.Label = "#VALX\n#VALY";
                    break;

                case "Column":
                    Chart1.ChartAreas[0].Area3DStyle.Inclination = 15;
                    Chart1.ChartAreas[0].Area3DStyle.Rotation = -15;
                    Chart1.ChartAreas[0].Area3DStyle.IsClustered = true;
                    Chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                    Chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                    //Chart1.ChartAreas(0).AxisX.LabelStyle.Angle = -45
                    Chart1.ChartAreas[0].AxisX.Interval = 1;
                    Chart1.Legends[0].Alignment = StringAlignment.Far;
                    Chart1.Legends[0].Docking = Docking.Top;
                    foreach (Series srs in Chart1.Series)
                        srs.IsValueShownAsLabel = true;
                    break;
            }
        }

        private void buildChart(Chart Chrt,int width,int height,int zoomed)
        {
            Chrt.ChartAreas.Clear();
            Chrt.Series.Clear();
            Chrt.Legends.Clear();
            Chrt.Width = width;
            Chrt.Height = height;
            Chrt.ChartAreas.Add("A");
            //if (zoomed==0)
            //{
            //    Chrt.ID = ID;
            //}
            //else
            //{
            //    Chrt.ID = ID+"zoomed";
            //}
            Chrt.ID = ID;
            Chrt.Titles.Add(Name);
            if (Subtitle != "") Chrt.Titles.Add(Subtitle);
            if (LegendLabels != "") Chrt.Legends.Add("L");


            foreach (KeyValuePair<string,string> sSer in cSeries)
            {
                Chrt.Series.Add(sSer.Value);
                srs = Chrt.Series[sSer.Value];
                if (LegendLabels != "")
                    try
                    {
                        srs.LegendText = getCollecationValue(cLegend, sSer.Value);
                    }
                    catch { }

                srs.ChartTypeName = ChrtType;

                if (cAxisTypes.Count>0)
                {
                    foreach (KeyValuePair<string,string> item in cAxisTypes)
                    {
                        if (sSer.Value == "A"+item.Key)
                        {
                            srs.YAxisType = (item.Value == "Primary" ? AxisType.Primary : AxisType.Secondary);
                        }
                    }
                }

                foreach (KeyValuePair<string,string> sLbl in cLabels)
                {

                    srs.Points.AddXY(sLbl.Value, getCollecationValue(cValues, sLbl.Value + sSer.Value));
                    if (Url != "")
                        setUrl(Url, sLbl.Value, sSer.Value, srs.Points.Last());
                }
            }
        }

        private void addMissingPoints()
        {
            foreach (KeyValuePair<string,string> sSer in cSeries)
                foreach (KeyValuePair<string, string> sLbl in cLabels)
                    try
                    {
                        double x = int.Parse(getCollecationValue(cValues, sLbl.Value + sSer.Value));
                    }
                    catch (Exception ex)
                    {
                        CollectionAdd(cSlabel, sLbl.Value, sLbl.Value + sSer.Value);
                        CollectionAdd(cValues, "0", sLbl.Value + sSer.Value);
                    }
        }

        private void readChartData()
        {

            // Read Data And Fill Points

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings[DataSourceID].ConnectionString;
            SqlConnection dbConnection = new System.Data.SqlClient.SqlConnection(connStr);
            SqlCommand cD = new SqlCommand(Query, dbConnection);
            dbConnection.Open();
            SqlDataReader dr = null;
            try
            {
                dr = cD.ExecuteReader();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "<br /><br />" + cD.CommandText);
                //r.Close()
                dbConnection.Close();
                Response.End();
            }

            if (LegendLabels != null) Chart1.Legends.Add("L");

            while (dr.Read())
            {
                string seriesName = "A" + (LegendLabels == "" ? "" : dr[LegendLabels]);
                try
                {
                    CollectionAdd(cSeries, seriesName, seriesName);
                    CollectionAdd(cLegend, dr[LegendLabels].ToString(), seriesName);
                }
                catch { }
                if (dr[Xlabels] != DBNull.Value)
                {
                    string sLbl = dr[Xlabels].ToString();
                    if (sLbl != "")
                    {
                        try
                        {
                            CollectionAdd(cLabels, sLbl, sLbl);
                        }
                        catch { }
                        //try
                        //{
                            CollectionAdd(cSlabel, sLbl, sLbl + seriesName);
                            double d = double.Parse(dr[Yvalues].ToString());
                            CollectionAdd(cValues, (dr[Yvalues] == DBNull.Value ? 0 : d).ToString() , sLbl + seriesName);
                            if (Url != "") CollectionAdd(cUrls, dr[Url].ToString(), sLbl + seriesName);
                        //}
                        //catch { }
                    }
                }
            }

            dr.Close();
            dbConnection.Close();

        }

        private void updateQueryParameters(int Id,int zoomed,int divId,int UserId)
        {
            Query = Query.Replace("{UserID}", "864");
            if (DetailedQuery!=null)
            {
                DetailedQuery = DetailedQuery.Replace("{UserID}", "864");
            }
            
            if (Params != null)
            {
                string[] p0 = Params.Split('|');
                for (int i = 0; i <= p0.Length - 1; i++)
                {
                    string thePlace = "{"+(i+1)+"}";
                    Query = Query.Replace(thePlace, p0[i]);
                    if (DetailedQuery!=null)
                    {
                        DetailedQuery = DetailedQuery.Replace(thePlace, p0[i]);
                    }                  
                    Name = Name.Replace(thePlace, p0[i]);
                    Subtitle = Subtitle.Replace(thePlace, p0[i]);
                }
            }
            if (zoomed == 1)
            {
                saveQuerString(divId, Id, 864, Query,DetailedQuery);
            }
        }

        private void readCharXML(int Id, int SubId)
        {

            // Open Source if not already openned
            
            if (xE == null) xE = XElement.Load(HttpContext.Current.Server.MapPath("~/" + XMLDataSource));

            // Read Chart Definition

            if (Id == 0) return;

            var q = from ll in xE.Descendants("Query")
                    where (ll.Parent.Attribute("ID").Value == Id.ToString()) && (ll.Parent.Attribute("SubID").Value == SubId.ToString("#"))
                    select new
                    {   
                        ChartId = ll.Parent.Attribute("ID").Value,//set Chart ID to recieved Id
                        DSid = ll.Attribute("DataSourceID").Value,
                        nam = ll.Parent.Attribute("Name").Value,
                        Subt = (ll.Parent.Attribute("SubTitle") == null ? "" : ll.Parent.Attribute("SubTitle").Value.ToString()),
                        Query = (ll != null ? ll.Value : ""),
                        LegendLabels = (ll.Attribute("LegandLabels") != null ? ll.Attribute("LegandLabels").Value.ToString() : ""),
                        XLabels = (ll.Attribute("XLabels") != null ? ll.Attribute("XLabels").Value : ""),
                        YValues = (ll.Attribute("YValues") != null ? ll.Attribute("YValues").Value : ""),
                        ChartType = (ll.Attribute("ChartType") != null ? ll.Attribute("ChartType").Value : ""),
                        Url = (ll.Attribute("Url") != null ? ll.Attribute("Url").Value : ""),
                        Trgt = (ll.Attribute("Target") != null ? ll.Attribute("Target").Value : "")
                    };
            foreach (var l in q)
            {
                ID = "Chart"+l.ChartId;////set Chart ID to recieved Id
                Name = l.nam;
                Query = (l.Query == null ? "" : l.Query);
                LegendLabels = (l.LegendLabels == null ? "" : l.LegendLabels);
                Xlabels = (l.XLabels == null ? "" : l.XLabels);
                Yvalues = (l.YValues == null ? "" : l.YValues);
                ChrtType = (l.ChartType == null ? "" : l.ChartType);
                DataSourceID = (l.DSid == null ? "" : l.DSid);
                Subtitle = l.Subt;
                Url = l.Url;
                Target = l.Trgt;
                break;
            }
            var q1 = from ll in xE.Descendants("srs")
                     where (ll.Parent.Parent.Attribute("ID").Value == Id.ToString()) && (ll.Parent.Parent.Attribute("SubID").Value == SubId.ToString("#"))
                     select new {
                         SrsName = (ll.Attribute("LegandLabel")==null?"": ll.Attribute("LegandLabel").Value),
                         AxisTyp = (ll.Attribute("axisType") == null ? "" : ll.Attribute("axisType").Value)
                     };
            foreach (var item in q1)
            {
                if (item.SrsName!="" && item.AxisTyp!="")
                {
                    cAxisTypes.Add(new KeyValuePair<string, string>(item.SrsName, item.AxisTyp));
                }
            }

            var dq = from ll in xE.Descendants("DetailsQuery")
                    where (ll.Parent.Attribute("ID").Value == Id.ToString()) && (ll.Parent.Attribute("SubID").Value == SubId.ToString("#"))
                    select new
                    {
                        //ChartId = ll.Parent.Attribute("ID").Value,//set Chart ID to recieved Id
                        //DSid = ll.Attribute("DataSourceID").Value,
                        //nam = ll.Parent.Attribute("Name").Value,
                        //Subt = (ll.Parent.Attribute("SubTitle") == null ? "" : ll.Parent.Attribute("SubTitle").Value.ToString()),
                        DetailedQuery = (ll != null ? ll.Value : ""),
                        //LegendLabels = (ll.Attribute("LegandLabels") != null ? ll.Attribute("LegandLabels").Value.ToString() : ""),
                        //XLabels = (ll.Attribute("XLabels") != null ? ll.Attribute("XLabels").Value : ""),
                        //YValues = (ll.Attribute("YValues") != null ? ll.Attribute("YValues").Value : ""),
                        //ChartType = (ll.Attribute("ChartType") != null ? ll.Attribute("ChartType").Value : ""),
                        //Url = (ll.Attribute("Url") != null ? ll.Attribute("Url").Value : ""),
                        //Trgt = (ll.Attribute("Target") != null ? ll.Attribute("Target").Value : "")
                    };
            foreach (var l in dq)
            {
                //ID = "Chart" + l.ChartId;////set Chart ID to recieved Id
                //Name = l.nam;
                DetailedQuery = (l.DetailedQuery == null ? "" : l.DetailedQuery);
                //LegendLabels = (l.LegendLabels == null ? "" : l.LegendLabels);
                //Xlabels = (l.XLabels == null ? "" : l.XLabels);
                //Yvalues = (l.YValues == null ? "" : l.YValues);
                //ChrtType = (l.ChartType == null ? "" : l.ChartType);
                //DataSourceID = (l.DSid == null ? "" : l.DSid);
                //Subtitle = l.Subt;
                //Url = l.Url;
                //Target = l.Trgt;
                break;
            }
            _queryString = Query;
            _detailedQuery = DetailedQuery;
        }

        protected void CollectionAdd( List<KeyValuePair<string,string>> c, string v, string k = "")
        {
            if (k != "")
            {
                foreach (KeyValuePair<string,string> item in c)
                {
                    
                    if (k == item.Key)
                    {
                        return;
                    }
                }
                c.Add(new KeyValuePair<string, string>(k, v));
            }
        }
        protected string getCollecationValue( List<KeyValuePair<string,string>> c, string k)
        {
            string r = "";
            foreach (KeyValuePair<string,string> item in c)
            {
                if (k == item.Key)
                {
                    r = item.Value;
                }
            }
            return r;
        }
        protected void setUrl(string sUrl, string sLbl, string sSer, DataPoint p)
        {

            try
            {
                sUrl = getCollecationValue(cUrls, sLbl + sSer);
                sUrl = sUrl.Replace("~", Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath);
                if (Target == "")
                    p.Url = sUrl;
                else
                {
                    p.Url = "javascript:void(0);";
                   // sUrl = "$(\"#Div3\").on(\"click\", function () { $(\"#Div3\").load('/grf.aspx?id=2' +'&sid=1&width=' + 300 + '&height=' + 300') })";

                //    sUrl = "onclick=" + "\"" + string.Format("{0:MM/dd/yyyy hh:mm:ss.fff tt}",System.DateTime.Now) + "\"";
                    sUrl = sUrl.Replace("\r\n", string.Empty);
                    sUrl = sUrl + "&width=800&height=600";
                   // sUrl = "onclick = \"window.open('" + sUrl + "','$(\"#Zooming\"');\""; --- working
                    sUrl = "onclick = \"DrillDown('" + sUrl + "');\"";
                    p.MapAreaAttributes = sUrl;
                    
                }
            }
            catch { }
        }
    }
}