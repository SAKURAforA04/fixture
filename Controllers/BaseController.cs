using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fixture02.Models;
using PagedList;

namespace Fixture02.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string controllerName = (filterContext.RouteData.Values["controller"]).ToString().ToLower();
            string actionName = (filterContext.RouteData.Values["action"]).ToString().ToLower();
            fixtureEntities db = new fixtureEntities();

            if (actionName != "login" && actionName!= "forgetpassword" && actionName!= "forgetpassword1")
            {
                base.OnActionExecuting(filterContext);
                Employee user = (Employee)System.Web.HttpContext.Current.Session["user"];

                if (user == null)
                {
                    filterContext.Result = new RedirectResult("~/Home/Login");
                    return;
                }
                

                if (user.UserLevel == "初级用户")
                {
                    string action = controllerName +"-"+ actionName;
                    var exist = db.Authorities.Where(a => a.junior.Contains(action)).ToArray().Length;
                    if(exist==1)
                    {
                        //filterContext.Result = new RedirectResult("~/Home/Login");
                        filterContext.Result = noAuthorty();
                        return;
                    }
                }
                if (user.UserLevel == "高级用户")
                {
                    string action = controllerName + "-" + actionName;
                    var exist = db.Authorities.Where(a => a.senior.Contains(action)).ToArray().Length;
                    if (exist == 1)
                    {
                        //filterContext.Result = new RedirectResult("~/Home/Login");
                        filterContext.Result = noAuthorty();
                        return;
                    }
                }
                if (user.UserLevel == "监管员")
                {
                    string action = controllerName + "-" + actionName;
                    var exist = db.Authorities.Where(a => a.supervisor.Contains(action)).ToArray().Length;
                    if (exist == 1)
                    {
                        //filterContext.Result = new RedirectResult("~/Home/Login");
                        filterContext.Result = noAuthorty();
                        return;
                    }
                }
                if (user.UserLevel == "部门经理")
                {
                    string action = controllerName + "-" + actionName;
                    var exist = db.Authorities.Where(a => a.manger.Contains(action)).ToArray().Length;
                    if (exist == 1)
                    {
                        //filterContext.Result = new RedirectResult("~/Home/Login");
                        filterContext.Result = noAuthorty();
                        return;
                    }
                }
                if (user.UserLevel == "系统管理员")
                {
                    string action = controllerName + "-" + actionName;
                    var exist = db.Authorities.Where(a => a.admin.Contains(action)).ToArray().Length;
                    if (exist == 1)
                    {
                        //filterContext.Result = new RedirectResult("~/Home/Login");
                        filterContext.Result = noAuthorty();
                        return;
                    }
                }
            }
        }

        public ActionResult noAuthorty()
        {
            return Content("抱歉，您没有访问此页面的权限，请联系管理员！");
        }
    }
}