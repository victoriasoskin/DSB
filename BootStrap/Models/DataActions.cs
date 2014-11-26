using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.DataVisualization;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using BootStrap.Controllers;

namespace BootStrap.Models
{
    public class DataActions
    {
        /// <summary>
        /// inserts the data from to DSB_UserPermissions.
        /// the user selects data in tahzuka and this function inserts the selected value to user permissions table
        /// the function recieves selectedDataId(int) 
        /// </summary>
        /// 
        XElement xE;
        public void InsertTo_Dsb_UserPermisions(int UserId, int selectedDataId, int DivId)
        {
            singleton s = new singleton();
            s.GetDBConnection();
            string sqlCheck = string.Format("select DataId from b10Sec.dbo.DSB_UserPermisions where userId = {0} and DivId={1}", UserId, DivId);
            int res = int.Parse(s.selectDBScalar(sqlCheck).ToString());
            if (res == null)
            {
                string sqlString = string.Format("insert into b10Sec.dbo.DSB_UserPermisions values ({0},{1},{2})", UserId, DivId, selectedDataId);
                s.executeSql(sqlString);
            }
            else
            {
                string sqlUpdate = string.Format("UPDATE B10Sec.dbo.DSB_UserPermisions SET DataId={0} WHERE userid={1} and DivId={2}", selectedDataId, UserId, DivId);
                s.executeSql(sqlUpdate);
            }

        }

        //public List <KeyValuePair<int,int>> ReturnUserPermissionsTable(double userId)
        //{
        //    DataTable dt = new DataTable();
        //    singleton s = new singleton();
        //    List <KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
        //    string sqlString = string.Format("select DivId,DataId,ViewId from B10Sec.dbo.DSB_UserPermisions where userid = {0} ", 864);
        //    dt = s.SelectDTQuery(sqlString);
        //    foreach(DataRow  item in dt.Rows)
        //    {
        //        pairs.Add(new KeyValuePair<int ,int>(int.Parse(item["DivId"].ToString()), int.Parse(item["DataId"].ToString())));
        //    }

        //    return pairs;
        //}

        public DataTable ReturnUserPermissionsTable(double userId)
        {
            DataTable dt = new DataTable();
            singleton s = new singleton();
            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
            string sqlString = string.Format("select DivId,DataId,ViewId,QueryToZoom from B10Sec.dbo.DSB_UserPermisions where userid = {0} ", 864);
            dt = s.SelectDTQuery(sqlString);
            return dt;
        }

        public List<KeyValuePair<int, int>> GetChartsTablesData(DataTable dt, int viewId)
        {
            DataActions da = new DataActions();
            List<KeyValuePair<int, int>> _pairs = new List<KeyValuePair<int, int>>();
            foreach (DataRow item in dt.Rows)
            {
                if (int.Parse(item["ViewId"].ToString()) == viewId)
                {
                    _pairs.Add(new KeyValuePair<int, int>(int.Parse(item["DivId"].ToString()), int.Parse(item["DataId"].ToString())));
                }

                // _pairs.Add(new KeyValuePair<int, int>(int.Parse(item["DivId"].ToString()), int.Parse(item["DataId"].ToString())));
            }
            return _pairs;
        }
        public resultObject ReturnChartId(int UserId, int source, int target)
        {
            singleton s = new singleton();
            resultObject res = new resultObject();
            string sqlString = string.Format("select DivId , DataId,ViewId from b10sec.dbo.DSB_UserPermisions where userid = {0} and DivId in ({1},{2})", UserId, source, target);
            DataTable dt = new DataTable();
            // int res = int.Parse(s.SelectDTQuery(sqlString).ToString());
            dt = s.SelectDTQuery(sqlString);
            if (int.Parse(dt.Rows[0][0].ToString()) == source)
            {
                res.SourceDataId = int.Parse(dt.Rows[0][1].ToString());
                res.ViewId = int.Parse(dt.Rows[0][2].ToString());
                if (dt.Rows.Count > 1)
                {
                    res.TargetDataId = int.Parse(dt.Rows[1][1].ToString());
                    res.ViewId = int.Parse(dt.Rows[1][2].ToString());
                }

            }
            else if (int.Parse(dt.Rows[0][0].ToString()) == target)
            {
                res.TargetDataId = int.Parse(dt.Rows[0][1].ToString());
                res.ViewId = int.Parse(dt.Rows[0][2].ToString());
                if (dt.Rows.Count > 1)
                {
                    res.SourceDataId = int.Parse(dt.Rows[1][1].ToString());
                    res.ViewId = int.Parse(dt.Rows[1][2].ToString());
                }

            }
            return res;
        }

        public void SwapDivs(int UserId, int DataId, int DivId, int ViewId)
        {
            singleton s = new singleton();
            resultObject res = new resultObject();
            string sqlString = string.Format("UPDATE B10Sec.dbo.DSB_UserPermisions SET DataId={0} WHERE userid={1} and DivId={2} and ViewId={3}", DataId, UserId, DivId, ViewId);
            s.executeSql(sqlString);
        }

        public DataTable SuppliersList(string sqlQuery)
        {
            singleton s = new singleton();
            DataTable dt = new DataTable();
            List<Suppliers> SuppList = new List<Suppliers>();
            string sqlQueryFixed = sqlQuery.Replace("&gt;", ">");
            dt = s.SelectDTQuery(sqlQueryFixed);
            return dt;
        }

        //***********************************************************************
        public string CreateHtmlTable(DataTable table)
        {
            int ColumnsNum = 0;
            string html = "<table><thead><tr>";
            foreach (DataColumn coln in table.Columns)
            {
                html += "<th>" + coln.ColumnName + "</th>";
                ColumnsNum++;
            }
            html += "</tr></thead><tbody>";
            int i = 0;
            foreach (DataRow dr in table.Rows)
            {
                html += "<tr>";
                while (i < ColumnsNum)
                {
                    html += "<td>" + dr[i] + "</td>";
                    i++;
                }
                i = 0;
                html += "</tr>";
            }
            html += "</tbody></table>";
            return html;// SuppList;
        }
        //************************************************************************
        // public List<CurrentDaySupports> CurrDaySupportsList(string sqlQuery)
        public DataTable CurrDaySupportsList(string sqlQuery)
        {
            singleton s = new singleton();
            DataTable dt = new DataTable();
            List<CurrentDaySupports> SuppList = new List<CurrentDaySupports>();

            string sqlQueryFixed = sqlQuery.Replace("&gt;", ">");
            dt = s.SelectDTQuery(sqlQueryFixed);
            dt = removeSpaces(dt);
            return dt;// SuppList;           
        }

        private DataTable removeSpaces(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                string str0;
                string str1;
                string str2;
                string str3;
                str0 = row[0].ToString().Replace("'", "").ToString();
                str1 = row[1].ToString().Replace("'", "''").ToString();
                str2 = row[2].ToString().Replace("'", "").ToString();
                str3 = row[3].ToString().Replace("'", "").ToString();
                //str0 = Regex.Replace(str0, @"<[^>]*(>|$)|&nbsp;|&zwnj;|&raquo;|&laquo;", "אין מידע");
                //string str4 = str1.Replace("&nbsp;", " ");
                //str2 = Regex.Replace(str2, @"<[^>]*(>|$)|&nbsp;|&zwnj;|&raquo;|&laquo;", "אין מידע");
                if (str1 == "")
                {
                    str1 = "אין מידע";
                }
                row[0] = str0.Replace("&nbsp;", " ");
                row[1] = str1.Replace("&nbsp;", "אין מידע");
                row[2] = str2.Replace("&nbsp;", " ");

            }
            return dt;
        }

        internal string ReadXmlFile(string XMLDataSource, KeyValuePair<int, int> pairs, int ViewId)
        {
            QueryClass qc = new QueryClass();
            string DivId = pairs.Key.ToString(); ;
            string _dataId = pairs.Value.ToString();
            string _queryString = "";
            if (ViewId == 2)
            {
                if (xE == null) xE = XElement.Load(HttpContext.Current.Server.MapPath(XMLDataSource));
                var t = from node in xE.Descendants("table")
                        where (node.Attribute("ID").Value == _dataId) //&& (ll.Parent.Attribute("SubID").Value == SubId.ToString("#"))
                        select new
                        {
                            DataId = node.Attribute("ID").Value,//set Chart ID to recieved Id
                            DataSourceId = node.Attribute("DataSourceID").Value,
                            _queryString = node.Attribute("Query").Value
                        };
                qc.Query = t.First()._queryString;
                saveQueryString(pairs, t.First()._queryString, ViewId);
                return t.First()._queryString;
            }
            else if (ViewId == 1)
            {
                if (xE == null) xE = XElement.Load(HttpContext.Current.Server.MapPath(XMLDataSource));
                var t = from node in xE.Descendants("Query")
                        where (node.Parent.Attribute("ID").Value == _dataId) //&& (ll.Parent.Attribute("SubID").Value == SubId.ToString("#"))
                        select new
                        {
                            DataId = (node != null ? node.Parent.Attribute("ID").Value : ""),//set Chart ID to recieved Id
                            DataSourceId = (node != null ? node.Attribute("DataSourceID").Value : ""),
                            _queryString = (node != null ? node.Value : "")
                        };
                qc.Query = t.First()._queryString;
                var t1 = from node in xE.Descendants("DetailsQuery")
                         where (node.Parent.Attribute("ID").Value == _dataId) //&& (ll.Parent.Attribute("SubID").Value == SubId.ToString("#"))
                         select new
                         {
                             //DataId = (node != null ? node.Parent.Attribute("ID").Value : ""),//set Chart ID to recieved Id
                             //DataSourceId = (node != null ? node.Attribute("DataSourceID").Value : ""),
                             _queryString = (node != null ? node.Value : "")
                         };
                qc.DetailsQuery = (t1.First()._queryString == null ? "" : t1.First()._queryString);
                saveZoomedQueryString(pairs, t.First()._queryString, t1.First()._queryString, ViewId);
                return t.First()._queryString;
            }
            else return null;

        }

        internal void saveQueryString(KeyValuePair<int, int> pairs, string queryString, int ViewId)
        {
            singleton s = new singleton();
            int DataId = pairs.Value;
            int DivId = pairs.Key;
            string _sql = queryString.Replace("'", "''");
            string sql = string.Format("update B10Sec.dbo.DSB_UserPermisions set QueryToZoom='{0}' where DivId={1} and DataId={2} and userId={3}  and ViewId={4}", _sql, DivId, DataId, 864, ViewId);
            s.executeSql(sql);
        }

        internal void saveZoomedQueryString(KeyValuePair<int, int> pairs, string _queryString, string _DetailedQuery, int ViewId)
        {
            singleton s = new singleton();
            int DataId = pairs.Value;
            int DivId = pairs.Key;
            _queryString = _queryString.Replace("'", "''");
            _DetailedQuery = _DetailedQuery.Replace("'", "''");
            string sql = string.Format("update B10Sec.dbo.DSB_UserPermisions set QueryToZoom='{0}',DetailedQuery='{5}' where DivId={1} and DataId={2} and userId={3}  and ViewId={4}", _queryString, DivId, DataId, 864, ViewId, _DetailedQuery);
            s.executeSql(sql);
        }

        internal resultObject ReturnTableId(int UserId, int source, int viewId)
        {
            singleton s = new singleton();
            resultObject res = new resultObject();
            string sqlString = string.Format("select DivId , DataId,ViewId,QueryToZoom from b10sec.dbo.DSB_UserPermisions where userid = {0} and DivId ={1} and viewID={2}", UserId, source, viewId);
            DataTable dt = new DataTable();
            // int res = int.Parse(s.SelectDTQuery(sqlString).ToString());
            dt = s.SelectDTQuery(sqlString);
            if (dt.Rows.Count > 0)
            {
                res.TargetDataId = int.Parse(dt.Rows[0][1].ToString());
                res.ViewId = int.Parse(dt.Rows[0][2].ToString());
                res.QueryToZoom = dt.Rows[0][3].ToString();
                res.SourceDivId = source.ToString();
            }
            return res;
        }

        internal string GetQueryString(string DivId, string ViewId, string DataId, int UserId, string QueryKind)
        {
            string _queryString;
            string sql = string.Format("select QueryToZoom from b10sec.dbo.DSB_UserPermisions where userid = {0} and DivId ={1} and DataId = {2} and viewID={3}", UserId, DivId, DataId, ViewId);
            string detSql = string.Format("select DetailedQuery from b10sec.dbo.DSB_UserPermisions where userid = {0} and DivId ={1} and DataId = {2} and viewID={3}", UserId, DivId, DataId, ViewId);
            singleton s = new singleton();
            if (QueryKind == "1")
            {
                _queryString = s.selectDBScalar(sql).ToString();
            }
            else//(QueryKind==2)
            {
                _queryString = s.selectDBScalar(detSql).ToString();
            }
            return _queryString;
        }



        internal int GetDivId(int dataId, int UserId)
        {
            singleton s = new singleton();
            string sql = string.Format("select divId from  B10Sec.dbo.DSB_UserPermisions where userid ={0} and dataid= {1} ", UserId, dataId);
            int res = int.Parse(s.selectDBScalar(sql).ToString());
            return res;
        }

        public string test()
        {
            string res = "";
            return res;
        }



        internal DataTable getSupportsOfSomeChosenSupporter(string supporterName)
        {
            singleton s = new singleton();
            DataTable dt = new DataTable();
            SupportersCurrentDaySupports cds = new SupportersCurrentDaySupports();

            string sql = string.Format("with CTE (formatedDay) as " +
"(select  case	when datename(dw,getdate())='sunday' then 'א'  " +
"when datename(dw,getdate())='monday' then 'ב' " +
"when datename(dw,getdate())='tuesday' then 'ג' " +
"when datename(dw,getdate())='wednesday' then 'ד' " +
"when datename(dw,getdate())='thursday' then 'ה' " +
"when datename(dw,getdate())='friday' then 'ו' end  ) " +
"SELECT w.dayid [יום],d.[CustomerId][ת.ז.],cl.custlastname [שם משפחה], " +
"cl.custfirstname [שם פרטי],c.name [תחום],c1.name [נושא],wp.purposeText[מטרה] " +
",d.[Text] [פירוט],[Text1] [יעדים],[Text2] [מדדים],[SupporterName] [ספק] " +
"FROM [Book10].[dbo].[TT_WP_Details]  d " +
"left outer join (select * from [Book10].[dbo].[TT_WP_Week]  where DayId like cast((select formatedDay from CTE) as nvarchar)) w on d.eventid=w.EventID and d.formid=w.FormID and isnull(d.detailsid,d.id)=w.DetailsId " +
"left outer join book10.dbo.tt_wp wp on wp.eventid=d.eventid and wp.formid=d.formid and isnull(wp.wpid,wp.id)=d.wpid " +
"left outer join B10Sec.dbo.tt_classes c on c.id =wp.rangeid " +
"left outer join B10Sec.dbo.tt_classes c1 on c1.id =wp.subjectid " +
"left outer join Book10_21.dbo.customerlist cl on cl.customerid=d.customerid " +
"where supportername like N'{0}' and w.dayid = cast((select formatedDay from CTE) as nvarchar) order by isnull(DayId,'ת') ", supporterName);
            dt = s.SelectDTQuery(sql);
            return dt;
        }

        internal DataTable getAllSupportsOfSomeChosenSupporter(string supporterName)
        {
            singleton s = new singleton();
            DataTable dt = new DataTable();
            SupportersCurrentDaySupports cds = new SupportersCurrentDaySupports();

            string sql = string.Format("with CTE (formatedDay) as " +
"(select  case	when datename(dw,getdate())='sunday' then 'א'  " +
"when datename(dw,getdate())='monday' then 'ב' " +
"when datename(dw,getdate())='tuesday' then 'ג' " +
"when datename(dw,getdate())='wednesday' then 'ד' " +
"when datename(dw,getdate())='thursday' then 'ה' " +
"when datename(dw,getdate())='friday' then 'ו' end  ) " +
"SELECT w.dayid [יום],d.[CustomerId][ת.ז.],cl.custlastname [שם משפחה], " +
"cl.custfirstname [שם פרטי],c.name [תחום],c1.name [נושא],wp.purposeText[מטרה] " +
",d.[Text] [פירוט],[Text1] [יעדים],[Text2] [מדדים],[SupporterName] [ספק] " +
" FROM [Book10].[dbo].[TT_WP_Details]  d " +
" left outer join  (select * from [Book10].[dbo].[TT_WP_Week] ) w on d.eventid=w.EventID and d.formid=w.FormID and isnull(d.detailsid,d.id)=w.DetailsId " +
" left outer join book10.dbo.tt_wp wp on wp.eventid=d.eventid and wp.formid=d.formid and isnull(wp.wpid,wp.id)=d.wpid " +
"  left outer join B10Sec.dbo.tt_classes c on c.id =wp.rangeid " +
"  left outer join B10Sec.dbo.tt_classes c1 on c1.id =wp.subjectid " +
"  left outer join Book10_21.dbo.customerlist cl on cl.customerid=d.customerid " +
"  where supportername like N'{0}'  " +
"  order by isnull(DayId,'ת') ", supporterName);
            dt = s.SelectDTQuery(sql);
            return dt;
        }

        internal DataTable GetAllSupports(string _queryString)
        {
            singleton s = new singleton();
            DataTable dt = new DataTable();
            //string sql = "select f.framename FrameName,cl.Name,isnull(det.supportername,'ללא שם ספק') Supporter,count(isnull(det.id,det.detailsid)) Count  " +
            //             "  from (select * from book10.dbo.TT_WP_Details  where loadtime > '01.09.2014') det  " +
            //             "  left outer join (select * from  book10_21.dbo.framelist ) f on f.frameid = det.frm_CatId " +
            //             "  left outer join b10sec.dbo.tt_classes cl on cl.id=det.helperid " +
            //             "  where f.frameid not in (122)  " +
            //             "  group by f.framename, det.supportername,cl.Name ";
            dt = s.SelectDTQuery(_queryString);
            return dt;
        }
    }
}