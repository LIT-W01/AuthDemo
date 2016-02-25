using AuthDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthDemo
{
    public class LoggedInActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var mgr = new UserManager(Properties.Settings.Default.ConStr);
            filterContext.Controller.ViewBag.User = mgr.GetUserByUsername(filterContext.HttpContext.User.Identity.Name);
        }
    }
}