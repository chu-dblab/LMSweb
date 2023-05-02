using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMSweb.ViewModel
{
    public class MissionCreateViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }

        [Display(Name = "任務編號")]
        public string MID { get; set; }

        [Display(Name = "任務名稱")]
        public string Name { get; set; }

        [Display(Name = "任務內容")]
        public string Contents { get; set; }

        [Display(Name = "任務開始時間")]
        public string StartDate { get; set; }

        [Display(Name = "任務結束時間")]
        public string EndDate { get; set; }
    }
}