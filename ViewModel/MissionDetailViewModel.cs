using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class MissionDetailViewModel
    {
        public string CourseID { get; set; }
        public string MissionID { get; set; }
        public string CourseName { get; set; }
        [Display(Name = "任務名稱")]
        public string Name { get; set; }
        [Display(Name = "任務內容")]
        public string Content { get; set; }
        [Display(Name = "開始時間")]
        public string StartDate { get; set; }
        [Display(Name = "結束時間")]
        public string EndDate { get; set; }

        //以下欄位之後可能會刪除
        public bool IsGoalSetting { get; set; }        
        public bool IsDrawing { get; set; }
        public bool IsCoding { get; set; }        
        public bool IsDiscuss { get; set; }
        public bool IsAssess { get; set; }
        public bool IsGReflect { get; set; }
        public bool IsReflect { get; set; }
        public bool Is_Journey { get; set; }
        public bool IsAddPeerAssessmemt { get; set; }
        public bool IsAddMetacognition { get; set; }
    }
}