using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSweb.ViewModel
{
    public class DrawingViewModel
    {
        public string CID { get; set; }
        public string CName { get; set; }
        public string MID { get; set; }
        public string MName { get; set; }
        public int GID { get; set; }
        public string GName { get; set; }
        public string End { get; set; }
        public string DrawingImgPath { get; set; }
        public bool IsDiscuss { get; set; }
        public bool IsUpDateSuccess { get; set; }

    }
}