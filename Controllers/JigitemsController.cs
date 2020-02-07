using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using Fixture02.Models;

namespace Fixture02.Controllers
{
    public class JigitemsController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: Jigitems
        public ActionResult Index()
        {
            var state = from m in this.db.Jigitem select m;
            state = state.Where(h => h.State.Equals("新增") || h.State.Equals("退回"));

            return View(state);
        }

        // GET: Jigitems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jigitem jigitem = db.Jigitem.Find(id);
            if (jigitem == null)
            {
                return HttpNotFound();
            }
            return View(jigitem);
        }

        // GET: Jigitems/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,Code,SeqID,BillNo,RegDate,Location,State,Pic,AddDate,AddUserID,AddUserName")] Jigitem jigitem)
        {
            if (ModelState.IsValid)
            {
                jigitem.UsedCount = 0;
                jigitem.State = "新增";
                jigitem.AddDate = DateTime.Now;
                jigitem.AddUserID = "010";
                jigitem.AddUserName = "李观星";

                db.Jigitem.Add(jigitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jigitem);
        }

        // GET: Jigitems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jigitem jigitem = db.Jigitem.Find(id);
            if (jigitem == null)
            {
                return HttpNotFound();
            }
            return View(jigitem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemID,Code,SeqID,BillNo,RegDate,UsedCount,Location,State,Pic,AddDate,AddUserID,AddUserName,FirstReviewDate,FirstReviewUserID,FirstReviewUserName,SecondReviewDate,SecondReviewUserID,SecondReviewUserName,WaitTime,BackNote")] Jigitem jigitem)
        {
            if (ModelState.IsValid)
            {
                jigitem.State = "新增";
                db.Entry(jigitem).State = EntityState.Modified;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jigitem);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jigitem jigitem = db.Jigitem.Find(id);
            if (jigitem == null)
            {
                return HttpNotFound();
            }
            return View(jigitem);
        }

        // POST: Jigitems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Jigitem jigitem = db.Jigitem.Find(id);
            db.Jigitem.Remove(jigitem);
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


        //初审管理

        public ActionResult FirstReview()
        {
            var state = from m in this.db.Jigitem select m;
            state = state.Where(h => h.State.Equals("新增"));

            return View(state);
        }

        [HttpPost]
        public ActionResult FirstReview(int id,string state)
        {
            Jigitem jigitem = db.Jigitem.Find(id);
            string backnote = Request["backNote"];

            if (state=="同意")
            {
                jigitem.State = "初审";
            }
            else
            {
                jigitem.State = "退回";
                jigitem.BackNote = backnote;
            }
            
            jigitem.FirstReviewDate = DateTime.Now;
            jigitem.FirstReviewUserID = "011";
            jigitem.FirstReviewUserName = "llggxx";
            

            db.SaveChanges();
            return RedirectToAction("FirstReview");
        }


        //终审管理
        public ActionResult SecondReview()
        {
            var state = from m in this.db.Jigitem select m;
            state = state.Where(h => h.State.Equals("初审"));

            return View(state);
        }

        [HttpPost]
        public ActionResult SecondReview(int id, string state)
        {
            Jigitem jigitem = db.Jigitem.Find(id);
            string backnote = Request["backNote"];

            if (state == "同意")
            {
                jigitem.State = "终审";
            }
            else
            {
                jigitem.State = "退回";
                jigitem.BackNote = backnote;
            }

            jigitem.SecondReviewDate = DateTime.Now;
            jigitem.SecondReviewUserID = "022";
            jigitem.SecondReviewUserName = "llggxx2";

            db.SaveChanges();
            return RedirectToAction("SecondReview");
        }

        //采购入库查询
        public ActionResult FixtureItemFind(String code, String location, String state)
        {
            var name = from m in this.db.Jigitem select m;
            if (!String.IsNullOrEmpty(code))
            {
                name = name.Where(h => h.Code.Contains(code));
            }
            if (!String.IsNullOrEmpty(location))
            {
                name = name.Where(h => h.Location.Contains(location));
            }
            if (!String.IsNullOrEmpty(state))
            {
                name = name.Where(h => h.State.Contains(state));
            }
            return View(name);
        }

        public ActionResult EditItem(int id,string state)
        {
            Jigitem jigitem = db.Jigitem.Find(id);

            if(state == "退回")
            {
                jigitem.State = "新增";
            }
            
            db.SaveChanges();
            return RedirectToAction("Edit");
        }


        //change the state of item
        public ActionResult changeItemState(int? itemid, string statename)
        {
            if (itemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jigitem jigitem = db.Jigitem.Find(itemid);
            if (jigitem == null)
            {
                return HttpNotFound();
            }
            jigitem.State = statename;
            db.SaveChanges();
            return null;
        }
    }
}
