using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class Execution
    {
        [Key]
        [Column(Order = 0)]
        public int GID { get; set; }
        [Key]
        [Column(Order = 1)]
        public string MID { get; set; }
        public string CurrentStatus { get; set; }

        [ForeignKey("GID")]
        public Group Group { get; set; }

        [ForeignKey("MID")]
        public Mission Mission { get; set; }
    }
}