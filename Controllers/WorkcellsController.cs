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
    public class WorkcellsController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: Workcells
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            return View(db.Workcell.ToList().OrderBy(x => x.WorkcellID).ToPagedList(page, pageSize));
        }

        // GET: Workcells/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Workcell workcell = db.Workcell.Find(id);
            if (workcell == null)
            {
                return HttpNotFound();
            }
            return View(workcell);
        }

        // GET: Workcells/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WorkcellID,WorkcellName")] Workcell workcell)
        {
            if (ModelState.IsValid)
            {
                db.Workcell.Add(workcell);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(workcell);
        }

        // GET: Workcells/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Workcell workcell = db.Workcell.Find(id);
            if (workcell == null)
            {
                return HttpNotFound();
            }
            return View(workcell);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WorkcellID,WorkcellName")] Workcell workcell)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workcell).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workcell);
        }

        // GET: Workcells/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Workcell workcell = db.Workcell.Find(id);
            if (workcell == null)
            {
                return HttpNotFound();
            }
            return View(workcell);
        }

        // POST: Workcells/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Workcell workcell = db.Workcell.Find(id);
            db.Workcell.Remove(workcell);
            db.SaveChanges();
            return RedirectToAction("Index");
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
