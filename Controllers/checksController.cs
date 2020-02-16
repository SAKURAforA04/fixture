using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Fixture02.Models;

namespace Fixture02.Controllers
{
    public class ChecksController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: checks
        public ActionResult Index()
        {
            var state = from m in this.db.Check select m;
            //state = state.Where(h => h.ScrapState.Equals("新增") || h.ScrapState.Equals("退回"));
            return View(state);
        }

        // GET: checks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Check check = db.Check.Find(id);
            if (check == null)
            {
                return HttpNotFound();
            }
            return View(check);
        }

        // GET: checks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: checks/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckID,ItemID,Code,Name,FamilyID,FamilyName,Model,PartNo,AddDate,AddUserID,AddUserName,CheckUserName,CheckType,CheckState,CheckResult1,CheckResult2,CheckResult3,CheckResult4,CheckResult5,CheckResult6,CheckResult7,CheckResult8,Problem,WorkcellID")] Check check)
        {
            if (ModelState.IsValid)
            {
                db.Check.Add(check);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(check);
        }

        // GET: checks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Check check = db.Check.Find(id);
            if (check == null)
            {
                return HttpNotFound();
            }
            return View(check);
        }

        // POST: checks/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CheckID,ItemID,Code,Name,FamilyID,FamilyName,Model,PartNo,AddDate,AddUserID,AddUserName,CheckUserName,CheckType,CheckState,CheckResult1,CheckResult2,CheckResult3,CheckResult4,CheckResult5,CheckResult6,CheckResult7,CheckResult8,Problem,WorkcellID")] Check check)
        {
            if (ModelState.IsValid)
            {
                db.Entry(check).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(check);
        }

        // GET: checks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Check check = db.Check.Find(id);
            if (check == null)
            {
                return HttpNotFound();
            }
            return View(check);
        }

        // POST: checks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Check check = db.Check.Find(id);
            db.Check.Remove(check);
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

        [HttpGet]
        //库存点检
        public ActionResult InStockCheck()
        {
            List<JigitemModel> jigitemModels = new List<JigitemModel>();
            jigitemModels = (from jigitem in db.Jigitem 
                             join jig in db.Jig on jigitem.Code equals jig.Code
                             select new JigitemModel()
                             {
                                 ItemID = jigitem.ItemID,
                                 JigitemCode = jigitem.Code + "-" + jigitem.SeqID,
                                 Code = jigitem.Code,
                                 SeqID = jigitem.SeqID,
                                 UsedCount = jigitem.UsedCount,
                                 Location = jigitem.Location,
                                 State = jigitem.State,
                                 WorkcellID = jigitem.WorkcellID,
                                 Name = jig.Name,
                                 FamilyID = jig.FamilyID,
                                 FamilyName = jig.FamilyName,
                                 Model = jig.Model,
                                 PartNo = jig.PartNo
                             }).ToList();
            jigitemModels=jigitemModels.Where(h => h.State.Equals("库存")).ToList();

            return View(jigitemModels);
        }

        [HttpPost]
        public ActionResult InStockCheck(string CheckType,string CheckState, string ItemID,string Code,string SeqID,string Name,string Model,string PartNo,string WorkcellID,string FamilyID,string FamilyName,
                                            string CheckResult1, string CheckResult2, string CheckResult3, string CheckResult4,
                                            string CheckResult5, string CheckResult6, string CheckResult7, string CheckResult8)
        {
            Check check = new Check();
            check.ItemID = int.Parse(ItemID);
            check.Code = Code;
            check.SeqID = int.Parse(SeqID);
            check.Name = Name;
            check.Model = Model;
            check.PartNo = PartNo;
            check.WorkcellID = WorkcellID;
            check.FamilyID = FamilyID;
            check.FamilyName = FamilyName;

            check.AddDate = DateTime.Now;
            check.AddUserID = "011";
            check.AddUserName = "llggxx";
            

            

            if (CheckType == "库存")
            {
                check.CheckType = "库存";
            }
            else if (CheckType == "出库")
            {
                check.CheckType = "出库";
            }
            else if (CheckType == "入库")
            {
                check.CheckType = "入库";
            }

            if (CheckState == "完成")
            {
                check.CheckState = "完成";
                check.CheckResult1 = "ok";
                check.CheckResult2 = "ok";
                check.CheckResult3 = "ok";
                check.CheckResult4 = "ok";
                check.CheckResult5 = "ok";
                check.CheckResult6 = "ok";
                check.CheckResult7 = "ok";
                check.CheckResult8 = "ok";
            }
            else if(CheckState == "维修")

            {
                check.CheckState = "维修";
                check.CheckResult1 = CheckResult1;
                check.CheckResult2 = CheckResult2;
                check.CheckResult3 = CheckResult3;
                check.CheckResult4 = CheckResult4;
                check.CheckResult5 = CheckResult5;
                check.CheckResult6 = CheckResult6;
                check.CheckResult7 = CheckResult7;
                check.CheckResult8 = CheckResult8;
            }


                db.Check.Add(check);
            db.SaveChanges();
            return RedirectToAction("InStockCheck");
        }

    }
}
