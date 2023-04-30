using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class CourseEditViewModel
    {
        [Display(Name = "課程名稱")]
        public string CourseName { get; set; }
        public int TestType { get; set; }
    }
}