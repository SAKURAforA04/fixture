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
    
    public partial class Employee
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserLevel { get; set; }
        public string WorkcellID { get; set; }
        public string Job { get; set; }
        public string JobDepartmentID { get; set; }
        public string PasswordForget { get; set; }
    
        public virtual Workcell Workcell { get; set; }
    }
}
