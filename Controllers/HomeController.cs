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
            var jigitem = from m in this.db.Jigitem select m;
            var repair1 = from k in this.db.repair select k;
            var scrap = from t in this.db.ScrapSet select t;
            Employee user = (Employee)Session["user"];
            if(user == null)
            {
                jigitem = null;
                repair1 = null;
                scrap = null;
            }
            else
            if (user.UserLevel == "初级员工")
            {
                jigitem = null;
                repair1 = null;
                scrap = null;
            }
            else if (user.UserLevel == "高级员工")
            {
                jigitem = null;
                repair1 = repair1.Where(p => p.RepairState == "新增");
                scrap = null;
            }
            else if (user.UserLevel == "监管员")
            {
                jigitem = jigitem.Where(p => p.State == "新增");
                repair1 = repair1.Where(p => p.RepairState == "新增");
                scrap = scrap.Where(p => p.ScrapState == "新增");
            }
            else if (user.UserLevel == "部门经理")
            {
                jigitem = jigitem.Where(p => p.State == "新增" || p.State == "初审");
                repair1 = repair1.Where(p => p.RepairState == "新增");
                scrap = scrap.Where(p => p.ScrapState == "新增" || p.ScrapState == "初审");
            }
            modelPage model = new modelPage();
            model.listJig = new List<Jigitem>();
            model.listRep = new List<repair>();
            model.listScr = new List<Scrap>();
            if(jigitem != null)
            foreach(var jigitem2 in jigitem)
            {
                model.listJig.Add(jigitem2);
            }
            if(repair1 != null)
            foreach(var repair2 in repair1)
            {
                model.listRep.Add(repair2);
            }
            if(scrap != null)
            foreach(var scrap2 in scrap)
            {
                 model.listScr.Add(scrap2);
             }
            return View(model);
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
                    System.Web.HttpContext.Current.Session.Add("workcellId", userID.WorkcellID);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

    }

    public class modelPage
    {
        public List<Jigitem> listJig { get; set; }

        public List<repair> listRep { get; set; }

        public List<Scrap> listScr { get; set; }
    }
}