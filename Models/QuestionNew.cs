using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class QuestionNew
    {
        // 題目編號
        [Key]
        public string QuestionNewID { get; set; }
        // 題目內容
        public string QContent { get; set; }
        // 題目類型
        public string QType { get; set; }
        
        // 隸屬課程
        public string CID { get; set; }
        // 題目類別
        public string EProcedureID { get; set; }
        public virtual ExperimentalProcedure ExperimentalProcedure { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<OptionNew> Options { get; set; }
    }
}