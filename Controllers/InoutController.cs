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
    public class InoutController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: Jigitems
        public ActionResult Index(String code, String name, String location , String model, String partNo)
        {
            
            var jigitem = from m in this.db.Jigitem select m;
            if (!String.IsNullOrEmpty(code))
            {
                jigitem = jigitem.Where(h => h.Code.Contains(code));
            }
            if (!String.IsNullOrEmpty(location))
            {
                jigitem = jigitem.Where(h => h.Location.Contains(location));
            }

            OutJigList outJigList = new OutJigList();
            outJigList.OutModelList = new List<OutJig>();
            foreach(var jigmodel in jigitem)
            {
                Jig jig = db.Jig.Find(jigmodel.Code);
                if(!String.IsNullOrEmpty(name) && jig.Name != name)
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(model) && jig.Model != model)
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(partNo) && jig.PartNo != partNo)
                {
                    continue;
                }
                outJigList.OutModelList.Add(
                    new OutJig { Code = jigmodel.Code, Name = jig.Name, Model = jig.Model, PartNo = jig.PartNo, State = jigmodel.State ,Location = jigmodel.Location,ItemID = jigmodel.ItemID}
                );
            }
          
            return View(outJigList);
        }
        public ActionResult InIndex(String code, String name, String location, String model, String partNo)
        {
            var jigitem = from m in this.db.Jigitem select m;
            jigitem = jigitem.Where(p => p.State == "出库" || p.State == "领用");
            if (!String.IsNullOrEmpty(code))
            {
                jigitem = jigitem.Where(h => h.Code.Contains(code));
            }
            if (!String.IsNullOrEmpty(location))
            {
                jigitem = jigitem.Where(h => h.Location.Contains(location));
            }

            OutJigList outJigList = new OutJigList();
            outJigList.OutModelList = new List<OutJig>();
            foreach (var jigmodel in jigitem)
            {
                Jig jig = db.Jig.Find(jigmodel.Code);
                if (!String.IsNullOrEmpty(name) && jig.Name != name)
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(model) && jig.Model != model)
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(partNo) && jig.PartNo != partNo)
                {
                    continue;
                }
                outJigList.OutModelList.Add(
                    new OutJig { Code = jigmodel.Code, Name = jig.Name, Model = jig.Model, PartNo = jig.PartNo, State = jigmodel.State, Location = jigmodel.Location, ItemID = jigmodel.ItemID }
                );
            }

            return View(outJigList);
        }
        // GET: Jigitems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inout inout = db.Inout.Find(id);
            if (inout == null)
            {
                return HttpNotFound();
            }
            return View(inout);
        }

        // GET: Jigitems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jigitems/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IinOutID,ItemID,ViaUserName,State,LineID,LineName,Code,Name,Location,Model,PartNo,FamilyID,FamilyName,AddDate,AddUserID,AddUserName,ReviewDate,ReviewUserID,ReviewUserName,SecondReviewDate")] Inout inout)
        {
            if (ModelState.IsValid)
            {
                db.Inout.Add(inout);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inout);
        }

        // GET: Jigitems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inout inout = db.Inout.Find(id);
            if (inout == null)
            {
                return HttpNotFound();
            }
            return View(inout);
        }

        // POST: Jigitems/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IinOutID,ItemID,ViaUserName,State,LineID,LineName,Code,Name,Location,Model,PartNo,FamilyID,FamilyName,AddDate,AddUserID,AddUserName,ReviewDate,ReviewUserID,ReviewUserName,SecondReviewDate")] Inout inout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inout).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inout);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inout inout = db.Inout.Find(id);
            if (inout == null)
            {
                return HttpNotFound();
            }
            return View(inout);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Inout inout = db.Inout.Find(id);
            db.Inout.Remove(inout);
            db.SaveChanges();
            return RedirectToAction("SearchInoutMessage");
        }

        // GET: Jigitems/Delete/5
        public ActionResult OutStorage(OutJig item)
        {
            return View(item);
        }

        // POST: Jigitems/Delete/5
        [HttpPost]
        public ActionResult OutStorage(Inout item)
        {
            item.AddDate = System.DateTime.Now;
            if(db.Inout.Select(e => e.IinOutID).Any())
            {
                int maxLinOutID = db.Inout.Select(e => e.IinOutID).Max();
                item.IinOutID = maxLinOutID + 1;
            }
            Jigitem jig = (from c in db.Jigitem
                             where c.ItemID == item.ItemID
                             select c).Single();
            jig.State = item.State;
            this.db.Inout.Add(
                item);
            this.db.SaveChanges();

            return RedirectToAction("Index");
        }

       
        public ActionResult InStorage(OutJig item)
        {

            return View(item);
        }

        // POST: Jigitems/Delete/5
        [HttpPost]
        public ActionResult InStorageToSql(Inout item)
        {
            item.AddDate = System.DateTime.Now;
            if ( db.Inout.Select(e => e.IinOutID).Any())
            {
                int maxLinOutID = db.Inout.Select(e => e.IinOutID).Max();
                item.IinOutID = maxLinOutID + 1;
            }
            Jigitem jig = (from c in db.Jigitem
                           where c.ItemID == item.ItemID
                           select c).Single();
            jig.State = "库存";
            this.db.Inout.Add(
                item);
            this.db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult SearchInoutMessage(String code, String name, String location, String model, String partNo)
        {
            var inoutitem = from m in this.db.Inout select m;
            if (!String.IsNullOrEmpty(code))
            {
                inoutitem = inoutitem.Where(h => h.Code.Contains(code));
            }
            if (!String.IsNullOrEmpty(location))
            {
                inoutitem = inoutitem.Where(h => h.Location.Contains(location));
            }
            if (!String.IsNullOrEmpty(model))
            {
                inoutitem = inoutitem.Where(h => h.Model.Contains(model));
            }
            if (!String.IsNullOrEmpty(name))
            {
                inoutitem = inoutitem.Where(h => h.Name.Contains(name));
            }
            if (!String.IsNullOrEmpty(partNo))
            {
                inoutitem = inoutitem.Where(h => h.PartNo.Contains(partNo));
            }

            return View(inoutitem);
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


    public class OutJig
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string PartNo { get; set; }
        public string State { get; set; }
        public string Location{ get; set; }
        public int ItemID { get; set; }
    }

    public class OutJigList
    {
        public List<OutJig> OutModelList{get; set;}
    }
}
