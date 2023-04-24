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
    [Authorize]
    public class HomeController : Controller
    {
        private LMSmodel db;
        public HomeController()
        {
            db = new LMSmodel();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel login)
        {
            var loginUser = db.Users
                .Where(u => u.ID == login.ID && u.UPassword == login.Password)
                .FirstOrDefault();

            if (loginUser != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(new[] {
                    //加入使用者的相關資訊
                    new Claim(ClaimTypes.Role, loginUser.RoleName),
                    new Claim(ClaimTypes.Name, loginUser.Name),
                    new Claim("UID",loginUser.ID)
                }, loginUser.RoleName);

                Request.GetOwinContext().Authentication.SignIn(identity); //授權(登入)
                return RedirectToAction("Home", loginUser.RoleName);
            }
            else
            {
                ModelState.AddModelError("","輸入的帳密可能有誤或是沒有註冊");
                return View(login);
            }            
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}