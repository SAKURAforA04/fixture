using fastJSON;
using Fixture02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fixture02.Controllers
{
    public class FlowController : Controller
    {
        // GET: Flow
        private fixtureEntities db = new fixtureEntities();
        public ActionResult Index(String code, String name, String location, String model, String partNo)
        {
            var flow = (from c in db.Inout where c.Code.Contains(code) select new flower { Code = c.Code,State = c.State ,Date = c.AddDate })
                .Union(from e in db.Jigitem where e.Code.Contains(code) select new flower { Code = e.Code, State = "库存", Date = e.AddDate })
                .Union(from e in db.repair where e.Code.Contains(code) select new flower { Code = e.Code, State = "报修", Date = e.AddDate })
                .Union(from e in db.repair where e.Code.Contains(code) select new flower { Code = e.Code, State = "库存", Date = e.RepairDate })
                .Union(from e in db.ScrapSet where e.Code.Contains(code) && e.ScrapState.Contains("报废")select new flower { Code = e.Code, State = "报废", Date = e.SecondReviewDate })
                .Union(from e in db.Check where e.Code.Contains(code) select new flower { Code = e.Code, State = "点检", Date = e.AddDate })
               .OrderBy(t => t.Date)
               ;

            FlowerList flowerList = new FlowerList();
            List<Nullable<System.DateTime>> Time = new List<Nullable<System.DateTime>>();
            List<int> State = new List<int>();
            flowerList.flowerModelList = new List<flower>();
            foreach (var flowermodel in flow)
            {
                flowerList.flowerModelList.Add(
                    new flower { Code = flowermodel.Code, State = flowermodel.Code, Date = flowermodel.Date}
                );

                Time.Add(flowermodel.Date);

                if(flowermodel.State == "库存")
                {
                    State.Add(1);
                }
                else if(flowermodel.State == "进库")
                        {
                    State.Add(2);
                }
                else if(flowermodel.State == "出库")
                        {
                    State.Add(3);
                }
                else if(flowermodel.State == "报修")
                {
                    State.Add(4);
                }
                else if (flowermodel.State == "点检")
                {
                    State.Add(5);
                }
                else if (flowermodel.State == "报废")
                {
                    State.Add(0);
                }
            }
            flowerList.time = JSON.ToJSON(Time).ToString();
            flowerList.state = JSON.ToJSON(State).ToString();
            return View(flowerList);
        }
    }


    public class flower
    {
        public string Code { get; set; }
        public string State { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
    public class FlowerList
    {
        public List<flower> flowerModelList { get; set; }

        public string time { get; set; }

        public string state { get; set; }
    }

}

