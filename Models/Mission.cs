using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSweb.Models
{
    [MetadataType(typeof(Mission))]
    public class Mission
    {
        [Key]
        [Display(Name = "任務編號")]
        public string MID { get; set; }
        [Required]
        [Display(Name = "任務名稱")]
        public string MName { get; set; }

        [Required]
        [Display(Name = "任務內容")]
        public string MDetail { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        public string Start { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "結束時間")]
        public string End { get; set; }

        //[Display(Name = "合作學習權重")]
        //public int discuss_k { get; set; }

        //[Display(Name = "個人表現權重")]
        //public int per_k {get;set;}

        //[Display(Name = "小組成果權重")]
        //public int group_k { get; set; }

        [Display(Name = "目標設置是否開放")]
        public bool IsGoalSetting { get; set; }

        [Display(Name = "流程圖是否開放")]
        public bool IsDrawing { get; set; }

        [Display(Name = "程式碼是否開放")]
        public bool IsCoding { get; set; }

        [Display(Name = "討論區是否開放")]
        public bool IsDiscuss { get; set; }

        [Display(Name = "自評與互評是否開放")]
        public bool IsAssess { get; set; }

        [Display(Name = "組間互評是否開放")]
        public bool IsGReflect { get; set; }

        [Display(Name = "學習表現自評版是否開放")]
        public bool IsReflect { get; set; }

        [Display(Name = "自我反思是否開放")]
        public bool Is_Journey { get; set; }

        [Display(Name = "課程編號")]
        public string CID { get; set; }

        public virtual Course Course { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<StudentMission> StudentMissions { get; set; }
        public virtual ICollection<LearningBehavior> LearningBehaviors { get; set; }
        public virtual ICollection<StudentCode> StudentCode { get; set; }
        public virtual ICollection<Question> Questions { get; set; }

    }
}