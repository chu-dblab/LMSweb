using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class Provided
    {
        [Key]
        [Column(Order = 0)]
        public string AID { get; set; }
        [Key]
        [Column(Order = 1)]
        public string StudentID { get; set; }

        public virtual Answer Answers { get; set; }
        public virtual Student Students { get; set; }
    }
}