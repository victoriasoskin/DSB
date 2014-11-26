using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootStrap.Models;
namespace BootStrap.Controllers
{
    public class DSBController : Controller
    {
        //
        // GET: /DSB/
        int DivId;
        public ActionResult Index()
        {
            singleton s = new singleton();
            DataTable dt = new DataTable();
            Charting res= new Charting();
            dt = res.SelectUserPermitions();

            List<DataSelection> UPList = new List<DataSelection>();
            //create dropdownList of all permissions of the user
            foreach (DataRow dr in dt.Rows)
            {
                DataSelection ds = new DataSelection();
                ds.DataId=int.Parse(dr["DataId"].ToString());
                ds.DataName = dr["DataName"].ToString();
                UPList.Add(ds);
            }
            @ViewBag.Permisions = new  SelectList(UPList, "DataId", "DataName");
            //Create();
            var y = int.Parse(Request.QueryString["DivId"].ToString());
            @ViewBag.DivId =y ;
            return View();
        }

        //[HttpPost]
        //public ActionResult Index()
        //{
        //  //  string result = id;
        //    //function

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

       

       
        public ActionResult DataSelect(resultObject result)
        {
            int DivId = result.DivId;
            int DataId = result.DataIdRes;
            DataActions Da = new DataActions();
            //int t = int.Parse(dataList[0]);
            Da.InsertTo_Dsb_UserPermisions(864,DataId, DivId);
            return View();
        }

        //
        // GET: /DSB/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DSB/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DSB/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DSB/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DSB/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DSB/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DSB/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
