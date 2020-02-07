using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fixture02.Models;

namespace Fixture02.Controllers
{
    public class EmployeesController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        
        public ActionResult EmployeeIndex(String searchString,String userLevel,String workcellID)
        {
            //var name = from m in this.db.Employee select m;
            var name = db.Employee.Include(e => e.Workcell);
            if (!String.IsNullOrEmpty(searchString))
            {
                name = name.Where(h => h.EmployeeName.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(userLevel))
            {
                name = name.Where(h => h.UserLevel.Contains(userLevel));
            }
            if (!String.IsNullOrEmpty(workcellID))
            {
                name = name.Where(h => h.WorkcellID.Contains(workcellID));
            }
            return View(name);
        }


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
    }
}
