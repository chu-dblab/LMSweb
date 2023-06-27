using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class Answer
    {
        [Key]
        // 作答編號
        public string AID { get; set; }
        // 作答內容
        public string AContent { get; set; }
        // 作答時間
        public DateTime ATime { get; set; }

        // 作答題目
        public string QuestionNewID { get; set; }

        [ForeignKey("QuestionNewID")]
        public virtual QuestionNew QuestionNew { get; set; }

        public virtual ICollection<Provided> Provideds { get; set; }
    }
}