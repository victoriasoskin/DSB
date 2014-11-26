using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BootStrap.Models;
using BootStrap.Controllers;

namespace BootStrap.Controllers
{
    public class APIController : ApiController
    {
        public string XMLDataSource { get; set; }

        // GET api/api
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/api/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/api
        public void Post([FromBody]string value)
        {
        }

        // PUT api/api/5
        public void Put(int id,resultObject result)
        {
            DataActions da = new DataActions();
            QueryClass qc = new QueryClass();
            string _queryString;
            if (result.ViewId==1)
            {
                XMLDataSource = "~/App_Data/DSBCharts.xml";
            }
            else if (result.ViewId==2)
            {
                XMLDataSource = "~/App_Data/DSBGrids.xml";
            }
            KeyValuePair<int, int> pair = new KeyValuePair<int, int>(int.Parse(result.SourceDivId.ToString()),int.Parse(result.SourceDataId.ToString()));
            
            _queryString = da.ReadXmlFile(XMLDataSource, pair, result.ViewId);
            da.saveQueryString(pair, _queryString, result.ViewId);
           // return _queryString;
        }

        // DELETE api/api/5
        public void Delete(int id)
        {
        }
    }
}
