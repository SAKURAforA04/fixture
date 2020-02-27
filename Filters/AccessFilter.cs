using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fixture02.Models;

namespace Fixture02.Filters
{
    public class AccessFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Employee user = (Employee)System.Web.HttpContext.Current.Session["user"];

            if(user == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
            return;
        }
    }
}