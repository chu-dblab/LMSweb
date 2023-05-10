using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class TeacherHomeViewModel
    {
        [Display(Name = "課程編號")]
        public string CourseID { get; set; }
        
        [Display(Name = "課程名稱")]
        public string CourseName { get; set; }
        
        [Display(Name = "實驗組別")]
        public int TestType { get; set; }
    }
}