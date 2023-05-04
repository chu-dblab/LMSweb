using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMSweb.Controllers
{
    public class ChartController : Controller
    {
        //patials view
        public ActionResult _ChartStackedBar()
        {
            TempData["Chart"] = "Chart";


            return PartialView();
        }
    }
}