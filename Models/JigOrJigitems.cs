using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fixture02.Models
{
    public class JigOrJigitems
    {
        public IEnumerable<Jig>  Jig { get; set; }
        public IEnumerable<Jigitem> Jigitem { get; set; }
        public JigOrJigitems()
        {
            fixtureEntities db = new fixtureEntities();
            this.Jig = db.Jig.ToList();
            this.Jigitem = db.Jigitem.ToList();
        }
    }
}