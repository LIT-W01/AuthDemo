using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AuthDemo.Data;

namespace AuthDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(string username, string password, string name)
        {
            var mgr = new UserManager(Properties.Settings.Default.ConStr);
            mgr.AddUser(username, password, name);
            return RedirectToAction("Signin");
        }

        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signin(string username, string password)
        {
            var mgr = new UserManager(Properties.Settings.Default.ConStr);
            var user = mgr.GetUser(username, password);
            if (user == null)
            {
                return View();
            }

            FormsAuthentication.SetAuthCookie(user.Username, true);
            return RedirectToAction("Secret");
        }

        [Authorize]
        public ActionResult Secret()
        {
            return View();
        }

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

    }
}
