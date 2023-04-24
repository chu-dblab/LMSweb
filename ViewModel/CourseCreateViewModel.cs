using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class CourseCreateViewModel
    {
        [Display(Name = "課程名稱")]
        [Required]
        public string Name { get; set; }
        [Required]
        public int TestType { get; set; }
    }
}