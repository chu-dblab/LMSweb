using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class OptionNew
    {
        // 選項編號
        [Key]
        public int OptionID { get; set; }
        public string OContent { get; set; }

        public string QuestionNewID { get; set; }

        [ForeignKey("QuestionNewID")]
        public virtual QuestionNew QuestionNew { get; set; }
    }
}