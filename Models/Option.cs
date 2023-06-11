using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class Option
    {
        // 選項編號
        [Key]
        public int OID { get; set; }
        public int QID { get; set; }

        // 題目選項
        [Display(Name = "選項")]
        public string OptionName { get; set; }


        public virtual Question Question { get; set; }
    }
}