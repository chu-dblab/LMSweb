using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class MissionIndexViewModel
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public List<MissionData> Missions { get; set; }
    }

    public class MissionData
    {
        [Display(Name = "任務編號")]
        public string MID { get; set; }

        [Display(Name = "任務名稱")]
        public string Name { get; set; }

        [Display(Name = "任務開始時間")]
        public string StartDate { get; set; }

        [Display(Name = "任務結束時間")]
        public string EndDate { get; set; }
    }
}