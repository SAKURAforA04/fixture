using Fixture02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fixture02.Controllers
{
    //public class HomeController : Controller
    //{
    //    Razor.Models.Product myProduct = new Models.Product { ProductID = 1, Name = "Book" };
    //    Razor.Models.Person myPerson = new Models.Person { PersonID = "1", Name = "Jack" };

    //    public ActionResult Index()
    //    {
    //        return View(Tuple.Create(myProduct, myPerson));  // 返回一个Tuple对象，Item1代表Product、Item2代表Person
    //    }
    //}

    public class HomeController : BaseController
    {
        private fixtureEntities db = new fixtureEntities();

        public ActionResult Index()
        {
            return View();
        }
        




        public ActionResult Login()
        {
            return View();   
        }



        public ActionResult MainHome()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Employee employee)
        {
            Employee userID = db.Employee.SingleOrDefault(n => n.EmployeeID == employee.EmployeeID);
            if(userID == null)
            {
                return Content("用户不存在");
            }
            else
            {
                if(employee.Password != userID.Password)
                {
                    return Content("密码错误");
                }
                else
                {
                    System.Web.HttpContext.Current.Session.Add("user", userID);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

    }
}