using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Fixture02.Models
{
    public class JigitemModel
    {
        public int ItemID { get; set; }
        public string JigitemCode { get; set; }
        //from Jigitem
        [DisplayName("夹具代码")]
        public string Code { get; set; }
        public int SeqID { get; set; }
        public Nullable<int> UsedCount { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public string WorkcellID { get; set; }
        //from Jig
        [DisplayName("夹具名称")]
        public string Name { get; set; }
        public string FamilyID { get; set; }
        public string FamilyName { get; set; }
        public string Model { get; set; }
        public string PartNo { get; set; }

    }
}