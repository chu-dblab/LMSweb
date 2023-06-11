using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class ExperimentalProcedure
    {
        [Key]
        // ExperimentalProcedure
        public string EProcedureID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<QuestionNew> Questions { get; set; }
    }
}