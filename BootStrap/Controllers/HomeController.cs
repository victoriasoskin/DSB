using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootStrap.Models;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;

using System.Drawing;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace BootStrap.Controllers
{
    public class HomeController : Controller
    {
        public string XMLDataSource { get; set; }
        //string CurrentDaySupports="";
        //string SupportersList = "";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        [HttpGet]
        public ActionResult DSB()
        {
            DataActions da = new DataActions();
            DataTable _dtGeneralData = new DataTable();
            _dtGeneralData = da.ReturnUserPermissionsTable(864);
            var _chartsResult = da.GetChartsTablesData(_dtGeneralData, 1); //1=charts
            var _tablesResult = da.GetChartsTablesData(_dtGeneralData, 2);//2=tables
            @ViewBag.Div1Container = _chartsResult[0].Value;
            @ViewBag.SourceDiv1 = 1;
            @ViewBag.Div2Container = _chartsResult[1].Value;
            @ViewBag.SourceDiv2 = 2;
            @ViewBag.Div3Container = _chartsResult[2].Value;
            @ViewBag.SourceDiv3 = 3;
            @ViewBag.Div4Container = _chartsResult[3].Value;
            @ViewBag.SourceDiv4 = 4;
            @ViewBag.Div5Container = _chartsResult[4].Value;
            @ViewBag.SourceDiv5 = 5;
            @ViewBag.Div6Container = _chartsResult[5].Value;
            @ViewBag.SourceDiv6 = 6;
            @ViewBag.Div7Container = _chartsResult[6].Value;
            @ViewBag.SourceDiv7 = 7;
            @ViewBag.Div8Container = _chartsResult[7].Value;
            @ViewBag.SourceDiv8 = 8;
            @ViewBag.Div11Container = _tablesResult[0].Value;
           
            SetTables(_tablesResult);
            SetChartQueries(_chartsResult);
            @ViewBag.suppliersQueryString = @ViewBag.suppliersQueryString1;
            var x = @ViewBag.suppliersQueryString;
            return View();
        }

        private void SetTables(List<KeyValuePair<int, int>> _chartsResult)
        {
            DataActions da = new DataActions();
            string _queryString;
            foreach (var item in _chartsResult)
            {
                int _dataId = item.Value;
                switch (_dataId)
                {
                    case 1: // 1 = List of suppliers in the frame or organization
                        _queryString = GetItemDataQueryXML(item, 2);
                        string SupportersList = _queryString;
                        DataTable dt1 = GetSuppliersList(SupportersList);
                        ViewBag.SupportersList = GetJson(dt1);
                        break;
                    case 2: //2= list of supports of current day for each supporter of the frame
                        _queryString = GetItemDataQueryXML(item, 2);
                        DataTable dt = GetCurrentDaySupports(_queryString);
                        ViewBag.currentDaySupportsHtmlTable =  da.CreateHtmlTable(dt); //GetJson(dt);
                        string t = ViewBag.currentDaySupportsHtmlTable;
                        ViewBag.CurrentDaySupportsQueyString = _queryString;
                        break;
                    case 4: // 3= List of all supports by supporter
                         _queryString = GetItemDataQueryXML(item, 2);
                        DataTable dt2 = GetAllSupports(_queryString);
                        ViewBag.AllSupports = da.CreateHtmlTable(dt2);

                        break;
                }

            }
        }

        private DataTable GetAllSupports(string _queryString)
        {
            List<CurrentDaySupports> _currDaySuppList = new List<CurrentDaySupports>();
            DataTable dt = new DataTable();
            DataActions da = new DataActions();
            dt = da.GetAllSupports(_queryString);

            return dt;
        }
        /// <summary>
        /// this method sets al the tables of the DSB from server
        /// gets only one parameter List of pairs (DivId,DataId) , 
        /// goes to XML file , gets the querystring and then creates datatable using that query  .
        /// now it doesnt uses the div\id to populate the Div , but in the future it will. 
        /// now the Div populites from the cient side using ViewBag created here.
        /// </summary>

        /// <param name="_tablesResult"></param>
        private void SetChartQueries(List<KeyValuePair<int, int>> _tablesResult)
        {
            string t = "";
            foreach (var item in _tablesResult)
            {
                 t= GetItemDataQueryXML(item,1);                        
            }
        }



        public string GetItemDataQueryXML(KeyValuePair<int, int> pairOfTableData, int ViewId)
        {
            string _queryString;
            if (ViewId==1)
            {
                XMLDataSource = "~/App_Data/DSBCharts.xml";
            }
            else if (ViewId==2)
            {
                XMLDataSource = "~/App_Data/DSBGrids.xml";
            }
           
            DataActions da = new DataActions();
            _queryString = da.ReadXmlFile(XMLDataSource, pairOfTableData, ViewId);
            return _queryString;
        }

        public DataTable GetSuppliersList(string SupportersList)
        {
            List<Suppliers> suppList = new List<Suppliers>();
            DataTable dt = new DataTable();
            DataActions da = new DataActions();
            dt = da.SuppliersList(SupportersList);
            return dt;
        }
        //the function returns all frames and all supporters
        public DataTable GetCurrentDaySupports(string supportsListQuery)
        {
            List<CurrentDaySupports> _currDaySuppList = new List<CurrentDaySupports>();
            DataTable dt = new DataTable();
            DataActions da = new DataActions();
            dt = da.CurrDaySupportsList(supportsListQuery);
            return dt;
        }

        //returns supportes of some specific supporter
        public string GetSupporterCurrentDaySupports(Supporter supp)
        {
            DataTable dt = new DataTable();
            DataActions da = new DataActions();
            dt = da.getSupportsOfSomeChosenSupporter(supp.SupporterName);
            ViewBag.supps = dt;
            string newTable =da.CreateHtmlTable(dt);
            return newTable;
        }

        public string GetAllSupportsOfSelectedSupporter(Supporter supp)
        {
            DataTable dt = new DataTable();
            DataActions da = new DataActions();
            dt = da.getAllSupportsOfSomeChosenSupporter(supp.SupporterName);
            ViewBag.supps = dt;
            string newTable = da.CreateHtmlTable(dt);
            return newTable;
        }

        public ActionResult FindChartId(resultObject result)
        {
            //string subStr = "";
            resultObject o = new resultObject();
            DataActions da = new DataActions();
            int source = 0;
            int target = 0;
            int _divId = 0;
            string _tableDiv = "#TableDiv";
            string _chartDiv = "#Div";
            resultObject res;
            if (result.SourceDivName.StartsWith(_tableDiv))
            {
                if (result.SourceDivId != null)
                {
                    source = int.Parse(o.Strip(result.SourceDivId, result.SourceDivName));
                    res = da.ReturnTableId(864, source, 2);
                    result.SourceDataId = res.TargetDataId;
                    result.ViewId = res.ViewId;
                    //result.SourceDivId = res.SourceDivId;
                }
                //return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if (result.SourceDivName.StartsWith(_chartDiv))
            {
                if (result.SourceDivId != null)
                {
                    source = int.Parse(o.Strip(result.SourceDivId, result.SourceDivName));
                }
                if (result.TargetDivId != null)
                {
                    target = int.Parse(o.Strip(result.TargetDivId, result.SourceDivName));
                }

                res = da.ReturnChartId(864, source, target);
                result.SourceDataId = res.SourceDataId;
                result.TargetDataId = res.TargetDataId;
                result.ViewId = res.ViewId;
                //return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        public resultObject FindChartIds(int UserId, resultObject result)
        {
            string subStr = "#Div";
            resultObject o = new resultObject();
            DataActions da = new DataActions();
            int source = int.Parse(o.Strip(result.SourceDivId, subStr));
            int target = int.Parse(o.Strip(result.TargetDivId, subStr));
            resultObject res = da.ReturnChartId(UserId, source, target);
            res.SourceDivId = source.ToString();
            res.TargetDivId = target.ToString();
            return res;
        }

        public ActionResult SwapDivs(resultObject result)
        {
            string subStr = "#Div";
            resultObject o = new resultObject();
            DataActions da = new DataActions();
            int source = int.Parse(o.Strip(result.SourceDivId, subStr));
            int target = int.Parse(o.Strip(result.TargetDivId, subStr));
            resultObject res = FindChartIds(864, result);

            da.SwapDivs(864, res.SourceDataId, target,1);
            da.SwapDivs(864, res.TargetDataId, source,1);
            result.SourceDataId = res.SourceDataId;
            result.SourceDivId = res.SourceDivId;
            result.TargetDataId = res.TargetDataId;
            result.TargetDivId = res.TargetDivId;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDivId(resultObject result)
        {
            string _tableDiv = "#TableDiv";
            string _chartDiv = "#Div";
            string s="";
            if (result.SourceDivName.StartsWith(_tableDiv))
            {
                s = result.SourceDivName.Replace(_tableDiv, "");
                result.ViewId = 2;
            }
            else if (result.SourceDivName.StartsWith(_chartDiv))
            {
                s = result.SourceDivName.Replace(_chartDiv, "");
                result.ViewId = 1;
            }
            result.SourceDivId = s;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string Strip(string s)
        {
            string _tableDiv = "#TableDiv";
            string _chartDiv = "#Div";

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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public string DbControl(DataTable dt)
        //{

        //        try
        //        {
        //            ViewBag.ResultData = GetJson(dt);
        //            ViewBag.ResultMessage = "Command executed successfully !";
        //        }
        //        catch (Exception ee)
        //        {
        //            ViewBag.ResultMessage = ee.ToString();
        //        }

        //        //return View();

        //}

        string GetJson(DataTable table)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;
            foreach (DataRow dataRow in table.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    row.Add(column.ColumnName.Trim(), dataRow[column]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);

        }


        
        //public ActionResult ExportToExcel(string temp)
        public ActionResult ExportToExcel( )
        {
            string DivId = Request.QueryString["DivId"].ToString();
            string ViewId = Request.QueryString["ViewId"].ToString();
            string DataId = Request.QueryString["DataId"].ToString();
            string QueryKind = Request.QueryString["QueryKind"].ToString();
            string _query = GetQueryString(DivId, ViewId, DataId, 864,QueryKind);     
            _query = _query.Replace("{UserId}", "864").Replace("{UserID}", "864").Replace("{userID}", "864").Replace("{userId}", "864");
            singleton s = new singleton();
            DataTable dt = new DataTable();
            dt = s.SelectDTQuery(_query);
            GridView grid = new GridView();
            grid.AutoGenerateColumns = false;
            grid.DataSource = dt;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.HeaderText = dt.Columns[i].ColumnName;
                bf.DataField = dt.Columns[i].ColumnName;
                grid.Columns.Add(bf);
            }
            grid.DataBind();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.Charset = "";
            System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            System.Web.HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            System.Web.HttpContext.Current.Response.Output.Write(sw.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
            return View();
        }

        private string GetQueryString(string DivId, string ViewId, string DataId, int UserId,string QueryKind)
        {
            DataActions da = new DataActions();
            string queryString = da.GetQueryString(DivId, ViewId, DataId, UserId,QueryKind);
            return queryString;
        }

        public ActionResult Print(string temp)
        {
            string _query = temp;
            Page p = new Page();

            singleton s = new singleton();
            DataTable dt = new DataTable();
            dt = s.SelectDTQuery(_query);
            DataRow dr = dt.NewRow();
            GridView grid = new GridView();
            grid.AutoGenerateColumns = false;
            grid.DataSource = dt;
            string d = dt.Rows[0][0] + " " + dt.Rows[0][1] + " " + dt.Rows[0][2] + " " + dt.Rows[0][3];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                BoundField bf = new BoundField();
                bf.DataField = dt.Columns[i].ColumnName;

                grid.Columns.Add(bf);
            }
            grid.AllowPaging = false;
            grid.DataBind();
            string str = grid.Rows[0].Cells[0].Text + " " + grid.Rows[0].Cells[1].Text + " " + grid.Rows[0].Cells[2].Text + " " + grid.Rows[0].Cells[3].Text;
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            string gridHTML = sw.ToString().Replace("\"", "'")
                .Replace(System.Environment.NewLine, "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload = new function(){");
            sb.Append("var printWin = window.open('', '', 'right=0");
            sb.Append(",top=0,width=1000,height=600,status=0');");
            sb.Append("printWin.document.write(\"");
            sb.Append("<table><thead><th><th>frameName</th><th>Supporter</th><th>CurrentDay</th><th>NumOfSupports</th> </tr></thead></table>");
            String style = "<style type = 'text/css'>table {display: direction:ltr; border:1px solid black;} th {border:1px solid black;font-family:Arial;font-color:Black} thead {display:table-header-group;} td {display:table-cell;direction:rtl;text-align:right;border:1px solid black;font-family:Arial;} .tdxprint {border:1px solid black;} .btn {color:Black;font-family:Arial;text-decoration:none;} .wp1 {width:0%;} .wp2 {width:30%;} .wp3 {width:45%;} .wp4 {width:25%;} .ansprint {color:Black;font-family:Arial; border:1px solid Black;} </style>";
            sb.Append(style + gridHTML);
            sb.Append("\");");
            sb.Append("printWin.document.close();");
            sb.Append("printWin.focus();");
            sb.Append("printWin.print();");
            sb.Append("printWin.close();");
            sb.Append("};");
            sb.Append("</script>");
            p.ClientScript.RegisterStartupScript(this.GetType(), "GridPrint", sb.ToString());
            ViewBag.ViewGrid = sb.ToString();
            //grid.AllowPaging = true;
            grid.DataBind();
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].HeaderText = dt.Columns[i].ColumnName;
                // grid.HeaderRow.Cells[i].Text=dt.Columns[i].ColumnName;
            }
            return View();
        }

    }
}