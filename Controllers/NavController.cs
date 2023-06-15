using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMSweb.ViewModel.NavPartial;

namespace LMSweb.Controllers
{
    public class NavController : Controller
    {
        // GET: Nav
        public ActionResult _TeacherNavPartial(TeacherNavViewModel vm)
        {
            return PartialView(vm);
        }
    }
}