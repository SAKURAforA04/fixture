using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fixture02.Models;

namespace Fixture02.Controllers
{
    public class JigOrJigitemsController : BaseController
    {
        public ActionResult Index()
        {
            JigOrJigitems all = new JigOrJigitems();
            return View(all);
        }
    }
}
