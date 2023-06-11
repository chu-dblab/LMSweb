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
        public string Content { get; set; }
        // 題目類型
        public string QType { get; set; }

        // 題目類別
        public string EProcedureID { get; set; }
        public virtual ExperimentalProcedure ExperimentalProcedure { get; set; }
    }
}