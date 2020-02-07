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
    public class JigsController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: Jigs
        public ActionResult Index()
        {
            return View(db.Jig.ToList());
        }

        // GET: Jigs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jig jig = db.Jig.Find(id);
            if (jig == null)
            {
                return HttpNotFound();
            }
            return View(jig);
        }

        // GET: Jigs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jigs/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Name,FamilyID,FamilyName,Model,PartNo,WorkcellID,WorkcellName,UsedFor,UPL,OwnerID,OwnerName,PMPeriod,AddDate,AddUserID,AddUserName,EditDate,EditUserID,EditUserName,Remark")] Jig jig)
        {
            if (ModelState.IsValid)
            {
                db.Jig.Add(jig);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jig);
        }

        // GET: Jigs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jig jig = db.Jig.Find(id);
            if (jig == null)
            {
                return HttpNotFound();
            }
            return View(jig);
        }

        // POST: Jigs/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code,Name,FamilyID,FamilyName,Model,PartNo,WorkcellID,WorkcellName,UsedFor,UPL,OwnerID,OwnerName,PMPeriod,AddDate,AddUserID,AddUserName,EditDate,EditUserID,EditUserName,Remark")] Jig jig)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jig).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jig);
        }

        // GET: Jigs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jig jig = db.Jig.Find(id);
            if (jig == null)
            {
                return HttpNotFound();
            }
            return View(jig);
        }

        // POST: Jigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Jig jig = db.Jig.Find(id);
            db.Jig.Remove(jig);
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
