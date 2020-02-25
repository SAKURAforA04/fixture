using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Fixture02.Models;
using PagedList;

namespace Fixture02.Controllers
{
    public class ChecksController : Controller
    {
        private fixtureEntities db = new fixtureEntities();

        // GET: checks
        public ActionResult Index(string code, string name, string checktype,string checkstate,
            int page = 1, int pageSize = 4)
        {
            var state = from m in this.db.Check select m;

            if (!String.IsNullOrEmpty(code))
            {
                state = state.Where(h => h.Code.Contains(code));
                ViewBag.code = code;
            }
            if (!String.IsNullOrEmpty(name))
            {
                state = state.Where(h => h.Name.Contains(name));
                ViewBag.name = name;
            }
            if (!String.IsNullOrEmpty(checktype))
            {
                state = state.Where(h => h.CheckType.Contains(checktype));
                ViewBag.checktype = checktype;
            }
            if (!String.IsNullOrEmpty(checkstate))
            {
                state = state.Where(h => h.CheckState.Contains(checkstate));
                ViewBag.checkstate = checkstate;
            }

            return View(state.OrderByDescending(x => x.CheckID).ToPagedList(page, pageSize));
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
        public ActionResult InStockCheck(string code, string name,string location,
            int page = 1, int pageSize = 4)
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

            if (!String.IsNullOrEmpty(code))
            {
                jigitemModels = jigitemModels.Where(h => h.Code.Contains(code)).ToList();
                ViewBag.code = code;
            }
            if (!String.IsNullOrEmpty(name))
            {
                jigitemModels = jigitemModels.Where(h => h.Name.Contains(name)).ToList();
                ViewBag.name = name;
            }
            if (!String.IsNullOrEmpty(location))
            {
                jigitemModels = jigitemModels.Where(h => h.Location.Contains(location)).ToList();
                ViewBag.location = location;
            }

            return View(jigitemModels.OrderBy(x => x.Code).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public ActionResult InStockCheck(string CheckType, string CheckState, string Pic,
            string ItemID, string Code, string SeqID, string Name, string Model, string PartNo, string WorkcellID, string FamilyID, string FamilyName,
            string ItemID1, string Code1, string SeqID1, string Name1, string Model1, string PartNo1, string WorkcellID1, string FamilyID1, string FamilyName1,
            string CheckResult1, string CheckResult2, string CheckResult3, string CheckResult4,
            string CheckResult5, string CheckResult6, string CheckResult7, string CheckResult8)
        {
            Check check = new Check();
            string problem="";

            if (CheckType == "库存")
            {
                check.CheckType = "库存";
                if (CheckState == "正常")
                {
                    check.CheckState = "正常";
                    check.ItemID = int.Parse(ItemID);
                    check.Code = Code;
                    check.SeqID = int.Parse(SeqID);
                    check.Name = Name;
                    check.Model = Model;
                    check.PartNo = PartNo;
                    check.WorkcellID = WorkcellID;
                    check.FamilyID = FamilyID;
                    check.FamilyName = FamilyName;
                    check.CheckResult1 = "false";
                    check.CheckResult2 = "false";
                    check.CheckResult3 = "false";
                    check.CheckResult4 = "false";
                    check.CheckResult5 = "false";
                    check.CheckResult6 = "false";
                    check.CheckResult7 = "false";
                    check.CheckResult8 = "false";
                }
                else
                {
                    check.CheckState = "维修";
                    check.ItemID = int.Parse(ItemID1);
                    check.Code = Code1;
                    check.SeqID = int.Parse(SeqID1);
                    check.Name = Name1;
                    check.Model = Model1;
                    check.PartNo = PartNo1;
                    check.WorkcellID = WorkcellID1;
                    check.FamilyID = FamilyID1;
                    check.FamilyName = FamilyName1;
                    check.CheckResult1 = CheckResult1;
                    check.CheckResult2 = CheckResult2;
                    check.CheckResult3 = CheckResult3;
                    check.CheckResult4 = CheckResult4;
                    check.CheckResult5 = CheckResult5;
                    check.CheckResult6 = CheckResult6;
                    check.CheckResult7 = CheckResult7;
                    check.CheckResult8 = CheckResult8;

                    if (CheckResult1=="true")
                    {
                        problem = problem + "夹具有螺丝松动。";
                    }
                    if (CheckResult2 == "true")
                    {
                        problem = problem + "夹具有部件损坏。";
                    }
                    if (CheckResult3 == "true")
                    {
                        problem = problem + "夹具有钢套损坏掉落等不良。";
                    }
                    if (CheckResult4 == "true")
                    {
                        problem = problem + "夹具有机械部件松动。";
                    }
                    if (CheckResult5 == "true")
                    {
                        problem = problem + "夹具电路没有正常工作。";
                    }
                    if (CheckResult6 == "true")
                    {
                        problem = problem + "夹具有不正常的响声。";
                    }
                    if (CheckResult7 == "true")
                    {
                        problem = problem + "夹具没有牢固的固定在对应位置。";
                    }
                    if (CheckResult8 == "true")
                    {
                        problem = problem + "没有使用抹布或者刷子进行夹具清洁。";
                    }
                    check.Problem = problem;
                    check.Pic = Pic;
                    //处理向维修表中插入点检出问题的夹具
                    //repair repair = new repair();

                    //repair.ItemID = int.Parse(ItemID1);
                    //repair.Code = Code1;
                    //repair.Name = Name1;
                    //repair.Model = Model1;
                    //repair.PartNo = PartNo1;
                    //repair.WorkcellID = WorkcellID1;
                    //repair.FamilyID = FamilyID1;
                    //repair.FamilyName = FamilyName1;

                    //repair.Problem = problem;
                    //repair.Pic = Pic;
                    //repair.AddDate = DateTime.Now;
                    //repair.RepairState = "新增";
                    ////repair.AddUserID = Session.user.userid;
                    ////repair.AddUserName = Session.user.username;
                    //repair.AddUserID = "Session.user.userid";
                    //repair.AddUserName = "Session.user.username";
                    //db.repair.Add(repair);
                }

            }
            
            check.AddDate = DateTime.Now;
            check.AddUserID = "011";
            check.AddUserName = "llggxx";
            
            db.Check.Add(check);

            
            db.SaveChanges();
            return RedirectToAction("InStockCheck");
        }

        [HttpGet]
        //库存点检
        public ActionResult OnlineCheck(
    int page = 1, int pageSize = 4)
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
            jigitemModels = jigitemModels.Where(h => h.State.Equals("库存")).ToList();



            return View(jigitemModels.OrderBy(x => x.ItemID).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public ActionResult OnlineCheck(string CheckType, string CheckState, string Pic,
            string ItemID, string Code, string SeqID,string Name, string Model, string PartNo, string WorkcellID, string FamilyID, string FamilyName, 
            string ItemID1, string Code1, string SeqID1,string Name1, string Model1, string PartNo1, string WorkcellID1, string FamilyID1, string FamilyName1,
            string CheckResult1, string CheckResult2, string CheckResult3, string CheckResult4,
            string CheckResult5, string CheckResult6, string CheckResult7, string CheckResult8)
        {
            Check check = new Check();

            if (CheckType == "库存")
            {
                check.CheckType = "库存";
                if (CheckState == "正常")
                {
                    check.CheckState = "正常";
                    check.ItemID = int.Parse(ItemID);
                    check.Code = Code;
                    check.SeqID = int.Parse(SeqID);
                    check.Name = Name;
                    check.Model = Model;
                    check.PartNo = PartNo;
                    check.WorkcellID = WorkcellID;
                    check.FamilyID = FamilyID;
                    check.FamilyName = FamilyName;
                    check.CheckResult1 = "false";
                    check.CheckResult2 = "false";
                    check.CheckResult3 = "false";
                    check.CheckResult4 = "false";
                    check.CheckResult5 = "false";
                    check.CheckResult6 = "false";
                    check.CheckResult7 = "false";
                    check.CheckResult8 = "false";
                }
                else
                {
                    check.CheckState = "维修";
                    check.ItemID = int.Parse(ItemID1);
                    check.Code = Code1;
                    check.SeqID = int.Parse(SeqID1);
                    check.Name = Name1;
                    check.Model = Model1;
                    check.PartNo = PartNo1;
                    check.WorkcellID = WorkcellID1;
                    check.FamilyID = FamilyID1;
                    check.FamilyName = FamilyName1;
                    check.CheckResult1 = CheckResult1;
                    check.CheckResult2 = CheckResult2;
                    check.CheckResult3 = CheckResult3;
                    check.CheckResult4 = CheckResult4;
                    check.CheckResult5 = CheckResult5;
                    check.CheckResult6 = CheckResult6;
                    check.CheckResult7 = CheckResult7;
                    check.CheckResult8 = CheckResult8;
                }

            }

            check.AddDate = DateTime.Now;
            check.AddUserID = "011";
            check.AddUserName = "llggxx";

            db.Check.Add(check);
            db.SaveChanges();
            return RedirectToAction("OnlineCheck");
        }








    }
}
