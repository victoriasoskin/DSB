using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using BootStrap;
using System.Data;


namespace BootStrap
{
    public class Charting
    {
        public DataTable SelectUserPermitions()
        {
            DataTable dt = new DataTable();
            singleton s = new singleton();
            string sql = "select DataName,DataId from b10Sec.dbo.DataNames";
            dt=s.SelectDTQuery(sql);
            return dt;
        }
    }
}
