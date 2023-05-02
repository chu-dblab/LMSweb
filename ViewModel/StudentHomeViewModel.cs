using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class StudentHomeViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public string GroupName { get; set; }
        public List<string> GroupMember { get; set; }
    }
}