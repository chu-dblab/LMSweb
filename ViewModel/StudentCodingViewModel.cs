using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class StudentCodingViewModel
    {
        public string SID { get; set; }
        public string CID { get; set; }
        public string CName { get; set; }
        public string MID { get; set; }
        public string MName { get; set; }
        public int GID { get; set; }
        public string GName { get; set; }
        public string End { get; set; }
        public string CodePath { get; set; }
        public string CodeText { get; set; }
        public bool IsEdit { get; set; }
        public string CodeContent { get; set; }
        public bool IsDiscuss { get; set; }
        public IEnumerable<LMSweb.Models.LearningBehavior> learningbehaviors { get; set; }
        public LMSweb.Models.LearningBehavior lbr { get; set; }
        public bool IsUpDateSuccess { get; set; }
    }
}