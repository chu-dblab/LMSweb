using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class ChangeLeaderViewModel
    {
        public string CourseID { get; set; }
        public string GroupName { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public bool IsLeader { get; set; }
    }
}