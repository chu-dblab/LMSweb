using LMSweb.Models;
using LMSweb.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace LMSweb.Controllers
{
    public class HomeController : Controller
    {
        private LMSmodel db;

        public HomeController()
        {
            db = new LMSmodel();
        }
        public ActionResult Index()
        {
            //teacher
            /*try
            {
                ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
                var claimData = claims.Claims.Where(x => x.Type == "TID").ToList();   //抓出當初記載Claims陣列中的TID
                var tid = claimData[0].Value; //取值(因為只有一筆)
                if (tid != null)

                return RedirectToAction("TeacherHomePage", "Teacher");
            }
            catch
            {

            }*/

            //student
            /*try
            {
                ClaimsIdentity claims = (ClaimsIdentity)User.Identity; //取得Identity
                var claimData = claims.Claims.Where(x => x.Type == "SID").ToList();   //抓出當初記載Claims陣列中的TID
                var sid = claimData[0].Value; //取值(因為只有一筆)
                if (sid != null)
                    return RedirectToAction("StudentHomePage", "Student");
            }
            catch
            {

            }*/

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}