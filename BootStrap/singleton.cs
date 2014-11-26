using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace BootStrap
{
    public class singleton
    {
        public static singleton instance;

        public singleton() { }

        public static singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new singleton();
                }
                return instance;
            }
        }
        public SqlConnection GetDBConnection()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BEBook10"].ConnectionString);
            // SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = "Data Source=82.80.209.137;Initial Catalog=Book10;Persist Security Info=True;User ID=sa;Password=karlosthe1st;Max Pool Size = 100;Pooling = True"/* providerName="System.Data.SqlClient"*/;
            // conn.ConnectionString="Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";  /*providerName="System.Data.SqlClient"*/
            //conn.Open();
            return conn;
        }
        public DataTable SelectDTQuery(string sqlString)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = GetDBConnection();
            conn.Open();
            if (conn == null)
            {
                throw new System.ArgumentException("The connection is closed");
            }
            SqlCommand sql = new SqlCommand(sqlString, conn);
            sql.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sql;
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        public object selectDBScalar(string sql)
        {
            SqlConnection cn = GetDBConnection();
            SqlCommand cD = new SqlCommand(sql, cn);
            cD.CommandType = CommandType.Text;
            cn.Open();
            object o = null;
            try
            {
                o = cD.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return o;
        }


        public Exception executeSql(string sql)
        {
            Exception eex = null;
            SqlConnection cn = GetDBConnection();
            DataTable dt = new DataTable();
            SqlCommand cD = new SqlCommand(sql, cn);
            cD.CommandType = CommandType.Text;
            cn.Open();
            object o = null;
            try
            {
                o = cD.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                eex = ex;
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return eex;

        }
        public XDocument GetXmlDocument()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DSBCharts.xml");
            XDocument doc = XDocument.Load(path);
            return doc;
        }
    }
}