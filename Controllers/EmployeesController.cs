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
    public class EmployeesController : BaseController
    {
        private fixtureEntities db = new fixtureEntities();

        public ActionResult EmployeeIndex(String employeeName, String userLevel, String workcellID, int page = 1, int pageSize = 4)
        {
            
            var name = db.Employee.Include(e => e.Workcell);
            if (!String.IsNullOrEmpty(employeeName))
            {
                name = name.Where(h => h.EmployeeName.Contains(employeeName));
                ViewBag.employeeName = employeeName;
            }
            if (!String.IsNullOrEmpty(userLevel))
            {
                name = name.Where(h => h.UserLevel.Contains(userLevel));
                ViewBag.userLevel = userLevel;
            }
            if (!String.IsNullOrEmpty(workcellID))
            {
                name = name.Where(h => h.WorkcellID.Contains(workcellID));
                ViewBag.workcellID = workcellID;
            }
            //用于分页，顺便跳转


            foreach(var item in name)
            {
                if(item.PasswordForget != null)
                {
                    System.Web.HttpContext.Current.Response.Write("<Script Language='JavaScript'>window.alert('" + "您有待处理的事项" + "');</script>");
                    List<Employee> forgets = new List<Employee>();
                    forgets.Add(item);
                    System.Web.HttpContext.Current.Session.Add("forgets", forgets);
                }
            }



            return View(name.OrderBy(x => x.EmployeeID).ToPagedList(page, pageSize));
        }

        //public ActionResult EmployeeIndex()
        //{
        //    return View(db.Employee.ToList());
        //}


        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            //增加下拉框显示部门名称
            ViewBag.WorkcellID = new SelectList(db.Workcell, "WorkcellID", "WorkcellName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmployeeName,Password,Email,UserLevel,WorkcellID,Job")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employee.Add(employee);
                db.SaveChanges();
                return RedirectToAction("EmployeeIndex");
            }
            //增加下拉框显示部门名称
            ViewBag.WorkcellID = new SelectList(db.Workcell, "WorkcellID", "WorkcellName", employee.WorkcellID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            //增加下拉框显示部门名称
            ViewBag.WorkcellID = new SelectList(db.Workcell, "WorkcellID", "WorkcellName", employee.WorkcellID);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmployeeName,Password,Email,UserLevel,WorkcellID,Job")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EmployeeIndex");
            }
            //增加下拉框显示部门名称
            ViewBag.WorkcellID = new SelectList(db.Workcell, "WorkcellID", "WorkcellName", employee.WorkcellID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = db.Employee.Find(id);
            db.Employee.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("EmployeeIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult forgetPassword()
        {
            return View();
        }

        public ActionResult forgetPassword1(Employee employee)
        {
            Employee userID = db.Employee.SingleOrDefault(n => n.EmployeeID == employee.EmployeeID);
            if (userID == null)
            {
                return Content("用户不存在");
            }
            else
            {
                if (employee.EmployeeName != userID.EmployeeName || employee.Email != userID.Email)
                {
                    return Content("您所输入的用户名或邮箱错误，请重试！");
                }
                else
                {
                    userID.PasswordForget = "yes";

                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult passwordReset(String id)
        {
            Employee emp = db.Employee.SingleOrDefault(n => n.EmployeeID == id);
            emp.Password = "123456";
            emp.PasswordForget = null;
            db.SaveChanges();
            return RedirectToAction("EmployeeIndex", "Employee",new { page = 1 });
        }
    }
}
