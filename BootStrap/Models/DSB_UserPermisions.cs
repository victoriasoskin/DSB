using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootStrap.Models
{
    public class DSB_UserPermisions
    {
        public int ID { get; set; }
        public long UserId { get; set; }
        public string DivId { get; set; }
        public string  DataName { get; set; }
    }
}