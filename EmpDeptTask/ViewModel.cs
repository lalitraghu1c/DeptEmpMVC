using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmpDeptTask
{
    public class DeptVM
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public string DeptLocation { get; set; }
        public string DeptJoining { get; set; }
        public string DeptLeaving { get; set; }
        public string StatueCode { get; set; }
        public string ResponseMessage { get; set; }
        public List<EmpVM> EmpVM { get; set; }
        public DeptVM()
        {
            EmpVM = new List<EmpVM>();
        }
    }

    public class EmpVM
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpCity { get; set; }
        public string EmpState { get; set; }
    }
}