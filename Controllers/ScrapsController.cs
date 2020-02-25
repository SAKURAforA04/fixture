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
    public class ScrapsController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: Scraps
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            //return View(db.ScrapSet.ToList());

            var state = from m in this.db.ScrapSet select m;
            state = state.Where(h => h.ScrapState.Equals("新增") || h.ScrapState.Equals("退回"));
            //加入分页，倒序ScrapID，最新增加的放最前面
            return View(state.OrderByDescending(x => x.ScrapID).ToPagedList(page, pageSize));
        }

        // GET: Scraps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scrap scrap = db.ScrapSet.Find(id);
            if (scrap == null)
            {
                return HttpNotFound();
            }
            return View(scrap);
        }

        // GET: Scraps/Create
        public ActionResult Create(int page = 1, int pageSize = 6)
        {
            List<JigitemModel> jigitemModels = new List<JigitemModel>();
            jigitemModels = (from jigitem in db.Jigitem
                             join jig in db.Jig on jigitem.Code equals jig.Code
                             select new JigitemModel()
                             {
                                 ItemID = jigitem.ItemID,
                                 JigitemCode = jigitem.Code + "-" + jigitem.SeqID,
                                 Code = jigitem.Code ,
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
            return View(jigitemModels.OrderBy(x => x.ItemID).ToPagedList(page, pageSize));
        }



        [HttpPost]
        public ActionResult Create([Bind(Include = "ScrapID,RepairID,ItemID,Code,Name,FamilyID,FamilyName,Model,PartNo,Count,Problem,AddDate,AddUserID,AddUserName,FirstReviewDate,FirstReviewUserID,FirstReviewUserName,SecondReviewDate,SecondReviewUserID,SecondReviewUserName,WorkcellID,ScrapState,BackNote")] Scrap scrap)
        {
            if (ModelState.IsValid)
            {
                scrap.ScrapState = "新增";
                scrap.AddDate = DateTime.Now;
                scrap.AddUserID = "010";
                scrap.AddUserName = "李观星";

                db.ScrapSet.Add(scrap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(scrap);
        }

        // GET: Scraps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scrap scrap = db.ScrapSet.Find(id);
            if (scrap == null)
            {
                return HttpNotFound();
            }
            return View(scrap);
        }

        // POST: Scraps/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ScrapID,RepairID,ItemID,Code,Name,FamilyID,FamilyName,Model,PartNo,Count,Problem,AddDate,AddUserID,AddUserName,FirstReviewDate,FirstReviewUserID,FirstReviewUserName,SecondReviewDate,SecondReviewUserID,SecondReviewUserName,WorkcellID,ScrapState,BackNote")] Scrap scrap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scrap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(scrap);
        }

        // GET: Scraps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scrap scrap = db.ScrapSet.Find(id);
            if (scrap == null)
            {
                return HttpNotFound();
            }
            return View(scrap);
        }

        // POST: Scraps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Scrap scrap = db.ScrapSet.Find(id);
            db.ScrapSet.Remove(scrap);
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


        //报废初审管理
        public ActionResult FirstScrap(int page = 1, int pageSize = 4)
        {
            var state = from m in this.db.ScrapSet select m;
            state = state.Where(h => h.ScrapState.Equals("新增"));

            return View(state.OrderBy(x => x.ScrapID).ToPagedList(page, pageSize));
        }
        [HttpPost]
        public ActionResult FirstScrap(int id, string state)
        {
            Scrap scrap = db.ScrapSet.Find(id);
            string backnote = Request["backNote"];
            if (state == "同意")
            {
                scrap.ScrapState = "初审";
            }
            else
            {
                scrap.ScrapState = "退回";
                scrap.BackNote = backnote;
            }
            scrap.FirstReviewDate = DateTime.Now;
            scrap.FirstReviewUserID = "011";
            scrap.FirstReviewUserName = "llggxx";
            db.SaveChanges();
            return RedirectToAction("FirstScrap");
        }

        //报废终审管理
        public ActionResult SecondScrap(int page = 1, int pageSize = 4)
        {
            var state = from m in this.db.ScrapSet select m;
            state = state.Where(h => h.ScrapState.Equals("初审"));

            return View(state.OrderBy(x => x.ScrapID).ToPagedList(page, pageSize));
        }
        [HttpPost]
        public ActionResult SecondScrap(int id, string state)
        {
            Scrap scrap = db.ScrapSet.Find(id);
            string backnote = Request["backNote"];
            if (state == "同意")
            {
                scrap.ScrapState = "已报废";
            }
            else
            {
                scrap.ScrapState = "退回";
                scrap.BackNote = backnote;
            }
            scrap.FirstReviewDate = DateTime.Now;
            scrap.FirstReviewUserID = "011";
            scrap.FirstReviewUserName = "llggxx";
            db.SaveChanges();
            return RedirectToAction("SecondScrap");
        }


        //报废查询
        public ActionResult FindScrap(String code, String state, int page = 1, int pageSize = 6)
        {
            var name = from m in this.db.ScrapSet select m;
            if (!String.IsNullOrEmpty(code))
            {
                name = name.Where(h => h.Code.Contains(code));
            }
            if (!String.IsNullOrEmpty(state))
            {
                name = name.Where(h => h.ScrapState.Contains(state));
            }
            return View(name.OrderBy(x => x.ScrapID).ToPagedList(page, pageSize));
        }

    }
}
