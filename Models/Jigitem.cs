//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fixture02.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Jigitem
    {
        [DisplayName("夹具实体流水号")]
        public int ItemID { get; set; }
        [DisplayName("夹具代码")]
        public string Code { get; set; }
        [DisplayName("夹具序列号")]
        public int SeqID { get; set; }
        [DisplayName("采购单据号")]
        public string BillNo { get; set; }
        [DisplayName("入库日期")]
        public System.DateTime RegDate { get; set; }
        [DisplayName("使用次数")]
        public Nullable<int> UsedCount { get; set; }
        [DisplayName("存放库位")]
        public string Location { get; set; }
        [DisplayName("状态")]
        public string State { get; set; }
        [DisplayName("夹具照片")]
        public string Pic { get; set; }
        [DisplayName("创建日期")]
        public System.DateTime AddDate { get; set; }
        [DisplayName("创建人编码")]
        public string AddUserID { get; set; }
        [DisplayName("创建人姓名")]
        public string AddUserName { get; set; }
        [DisplayName("初审日期")]
        public Nullable<System.DateTime> FirstReviewDate { get; set; }
        [DisplayName("初审人编码")]
        public string FirstReviewUserID { get; set; }
        [DisplayName("初审人姓名")]
        public string FirstReviewUserName { get; set; }
        [DisplayName("终审日期")]
        public Nullable<System.DateTime> SecondReviewDate { get; set; }
        [DisplayName("终审人编码")]
        public string SecondReviewUserID { get; set; }
        [DisplayName("终审人姓名")]
        public string SecondReviewUserName { get; set; }
        [DisplayName("缓存区时间")]
        public Nullable<System.DateTime> WaitTime { get; set; }
        [DisplayName("退回原因")]
        public string BackNote { get; set; }
        [DisplayName("工作部门")]
        public string WorkcellID { get; set; }
        [DisplayName("最后点检日期")]
        public Nullable<System.DateTime> FinalCheckDate { get; set; }
    
        public virtual Jig Jig { get; set; }
    }
}
