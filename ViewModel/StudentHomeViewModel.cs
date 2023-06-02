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
        public List<GroupStudentHomeViewModel> GroupStudent { get; set; }
    }

    public class GroupStudentHomeViewModel
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public bool IsLeader { get; set; }
    }
}