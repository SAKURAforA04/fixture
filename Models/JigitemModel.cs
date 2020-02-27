using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Fixture02.Models
{
    public class JigitemModel
    {
        [DisplayName("夹具实体流水号")]
        public int ItemID { get; set; }
        [DisplayName("夹具代码")]
        public string JigitemCode { get; set; }
        //from Jigitem
        [DisplayName("夹具代码")]
        public string Code { get; set; }
        [DisplayName("夹具序列号")]
        public int SeqID { get; set; }
        [DisplayName("使用次数")]
        public Nullable<int> UsedCount { get; set; }
        [DisplayName("存放库位")]
        public string Location { get; set; }
        [DisplayName("状态")]
        public string State { get; set; }
        [DisplayName("工作部门")]
        public string WorkcellID { get; set; }
        //from Jig
        [DisplayName("夹具名称")]
        public string Name { get; set; }
        [DisplayName("大类编码")]
        public string FamilyID { get; set; }
        [DisplayName("大类名称")]
        public string FamilyName { get; set; }
        [DisplayName("模组")]
        public string Model { get; set; }
        [DisplayName("料号")]
        public string PartNo { get; set; }

    }
}